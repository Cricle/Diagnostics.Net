﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Diagnostics.Generator.Internal
{
    internal static class ActivityMapParseHelper
    {
        public static bool TryWriteActivityMapCode(SourceProductionContext context, AttributeData mapToActivityAttr, ISymbol targetSymbol, IEnumerable<IMethodSymbol> methods, SemanticModel model, bool logMode, out string? code)
        {
            //var attr = targetSymbol.GetAttribute(Consts.MapToActivityAttribute.FullName);
            var symbol = (INamedTypeSymbol)mapToActivityAttr!.GetByIndex<ISymbol>(0)!;
            var withCallTog = mapToActivityAttr!.GetByNamed<bool>(Consts.MapToActivityAttribute.WithEventSourceCall);
            var callEventAtEnd = mapToActivityAttr!.GetByNamed<bool>(Consts.MapToActivityAttribute.CallEventAtEnd);

            var source = (INamedTypeSymbol)targetSymbol;

            var hasAccesstor = false;
            var instanceAccesstCode = string.Empty;
            if (withCallTog)
            {
                var sourceEventSourceGenerateAttr = source.GetAttribute(Consts.EventSourceGenerateAttribute.FullName);
                var isGenerateInstance = false;
                if (sourceEventSourceGenerateAttr != null)
                {
                    isGenerateInstance = sourceEventSourceGenerateAttr.GetByNamed<bool>(Consts.EventSourceGenerateAttribute.GenerateSingleton);
                }
                if (isGenerateInstance)
                {
                    instanceAccesstCode = $"global::{source}.Instance";
                    hasAccesstor = true;
                }
                else
                {
                    var accessInstance = source.GetMembers()
                        .Where(x => x is IFieldSymbol || x is IPropertySymbol)
                        .Where(x => x.HasAttribute(Consts.EventSourceAccesstorInstanceAttribute.FullName))
                        .FirstOrDefault();
                    if (accessInstance != null)
                    {
                        instanceAccesstCode = $"global::{source}.{accessInstance.Name}";
                        hasAccesstor = true;
                    }
                }
            }

            if (!hasAccesstor && withCallTog)
            {
                code = null;
                context.ReportDiagnostic(Diagnostic.Create(Messages.NoAccesstorNoCallTogether, source.Locations[0], source.Name));
                return false;
            }
            var eventSourceEvents = methods;

            var nullableEnable = symbol.GetNullableContext(model);
            var visibility = ParserBase.GetVisiblity(symbol);

            ParserBase.GetWriteNameSpace(symbol, model, out var nameSpaceStart, out var nameSpaceEnd);

            var fullName = ParserBase.GetTypeFullName(symbol);
            var @namespace = ParserBase.GetTypeFullName(symbol);
            if (!string.IsNullOrEmpty(@namespace))
            {
                @namespace = "global::" + @namespace;
            }

            var generateWithLog = mapToActivityAttr!.GetByNamed<bool>(Consts.MapToActivityAttribute.GenerateWithLog);

            code = @$"
#pragma warning disable CS8604
#nullable enable
            {nameSpaceStart}
                [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventSourceAttribute(typeof(global::{source.ToString().TrimEnd('?')}),{eventSourceEvents.Count()})]
                {visibility} partial class {symbol.Name}
                {{
                    {string.Join("\n", eventSourceEvents.Select(x => WriteMethodMap(symbol.IsStatic, x, model, withCallTog, instanceAccesstCode, callEventAtEnd, logMode, generateWithLog)))}
                }}
            {nameSpaceEnd}
#nullable restore
#pragma warning restore
                ";

            code = Helpers.FormatCode(code);
            return true;
        }

        private static string WriteMethodMap(bool isStatic, IMethodSymbol method, SemanticModel model, bool withCallTog, string eventSource, bool callEventAtEnd, bool logMode, bool generateWithLog)
        {
            var visibility = ParserBase.GetAccessibilityString(method.DeclaredAccessibility);
            var staticKeyword = isStatic ? "static" : string.Empty;
            var tags = method.GetAttributes(Consts.ActivityTagAttribute.FullName);
            var isNoEvent = method.HasAttribute(Consts.ActivityNoEventAttribute.FullName);
            var setStatusAttr = method.GetAttribute(Consts.ActivityStatusAttribute.FullName);
            var setStatus=setStatusAttr?.GetByIndex<int>(0);
            if (tags.Count == 0 && isNoEvent)
            {
                return string.Empty;
            }
            string? eventTagCodes;
            var pars = method.Parameters;
            if (logMode)
            {
                pars = pars.Skip(1).ToImmutableArray();
            }
            var parsActual = pars.Where(x => !x.HasAttribute(Consts.ActivityIgnoreAttribute.FullName)).ToImmutableArray();
            if (parsActual.Length != 0)
            {
                var methodArgs = string.Join("\n", parsActual.Select(x => $"tags[\"{x.Name}\"] = {x.Name};"));
                eventTagCodes = $@"
tags = new global::System.Diagnostics.ActivityTagsCollection();
{methodArgs}
if(additionTags != null)
{{
    foreach(global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?> tag in additionTags)
    {{
         if (tag.Key != null)
        {{
            tags[tag.Key] = tag.Value;
        }}
    }}
}}
";
            }
            else
            {
                eventTagCodes = @"
if(additionTags != null)
{
    tags = new global::System.Diagnostics.ActivityTagsCollection(additionTags);
}
";
            }
            var inputs = pars.Select(x => x.ToString());
            var inputJoined = string.Join(",", inputs);
            var additionArgs = string.Empty;
            var invokeArgsJoined = string.Empty;
            var hasPars = pars.Length != 0;
            if (hasPars)
            {
                additionArgs = $"{inputJoined},";
                invokeArgsJoined = string.Join(",", pars.Select(x => x.Name)) + ",";
            }
            var tagCodes = string.Empty;
            if (tags.Count != 0)
            {
                var s = new StringBuilder();
                foreach (var tag in tags)
                {
                    var tagMethod = "SetTag";
                    var name = tag.GetByIndex<string>(0);
                    var expression = tag.GetByIndex<string>(1);
                    var isSet = tag.GetByNamed<bool?>(Consts.ActivityTagAttribute.IsSet);
                    if (!isSet.GetValueOrDefault())
                    {
                        tagMethod = "AddTag";
                    }
                    if (pars.Length != 0)
                    {
                        s.AppendLine($"activity.{tagMethod}(\"{name}\",global::System.String.Format(\"{expression}\",{string.Join(",", pars.Select(x => x.Name))}));");
                    }
                    else
                    {
                        s.AppendLine($"activity.{tagMethod}(\"{name}\",\"{expression}\");");
                    }
                }
                tagCodes = s.ToString();
            }

            var endsArgs = $"{additionArgs} global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null,[global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null,[global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null,[global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0";

            var eventSourceParInvokeJoinedNoConvert = string.Join(",", pars.Select(x => x.Name));
            var eventSourceParInvokeJoined = string.Join(",", pars.Select(x =>
            {
                if (EventSourceHelper.IsExceptionType(model, x.Type))
                {
                    return $"{x.Name}?.ToString()";
                }
                return x.Name;
            }));

            var invokeCodesNoConvert = "timestamp,additionTags";
            var invokeCodes = invokeCodesNoConvert;
            if (pars.Length != 0)
            {
                invokeCodes = eventSourceParInvokeJoined + "," + invokeCodes;
                invokeCodesNoConvert = eventSourceParInvokeJoinedNoConvert + "," + invokeCodesNoConvert;
            }
            //Debugger.Launch();
            var loggerMessageAttr = method.GetAttribute(Consts.LoggerMessageAttribute.FullName);
            var logMessageData = LoggerMessageData.FromAttribute(method.Name, loggerMessageAttr, method.GetAttribute(Consts.EventAttribute.FullName));
            var eventId = logMessageData.EventId;
            var activityMapToEventAttr = $"[global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute({eventId},\"{method.Name}\",new global::System.Type[]{{ {string.Join(",", pars.Select(x => $"typeof({x.Type.ToString().TrimEnd('?')})"))} }})]";
            var argCode = $"global::System.Diagnostics.Activity? activity, {endsArgs}";

            var callEventSourceCodes = string.Empty;
            if (withCallTog)
            {
                callEventSourceCodes = @$"
if({eventSource}.IsEnabled())
{{
    {eventSource}.{method.Name}({eventSourceParInvokeJoined});
}}
";
            }
            var beginCallEventSourceCodes = string.Empty;
            var endCallEventSourceCodes = string.Empty;
            if (callEventAtEnd)
            {
                endCallEventSourceCodes = callEventSourceCodes;
            }
            else
            {
                beginCallEventSourceCodes = callEventSourceCodes;
            }

            var withLogCode = string.Empty;

            if (logMode && generateWithLog)
            {
                var noActivityArgs = endsArgs;
                var invokeCode = invokeArgsJoined.TrimEnd(',');
                if (hasPars)
                {
                    invokeCode = "," + invokeCode;
                }
                var invokeLog = $"global::{method.ContainingType}.{method.Name}(logger{invokeCode});";

                withLogCode = $@"
{activityMapToEventAttr}
{visibility} {staticKeyword} {method.ReturnType} {method.Name}(global::Microsoft.Extensions.Logging.ILogger? logger,{noActivityArgs})
{{
    if(logger!=null)
        {invokeLog}
    {method.Name}(global::System.Diagnostics.Activity.Current,{invokeCodesNoConvert});
}}
{activityMapToEventAttr}
{visibility} {staticKeyword} {method.ReturnType} {method.Name}(global::Microsoft.Extensions.Logging.ILogger? logger,{argCode})
{{
    if(logger!=null)
        {invokeLog}
    {method.Name}(activity,{invokeCodesNoConvert});
}}
";
            }

            string specialTagCodes = string.Empty;
            var specialAttr = method.GetAttribute(Consts.ActivitySpecialValueAttribute.FullName);
            if (specialAttr != null)
            {
                var hasFilePath = specialAttr.GetByNamed<bool>(Consts.ActivitySpecialValueAttribute.FilePath);
                var hasMemberName = specialAttr.GetByNamed<bool>(Consts.ActivitySpecialValueAttribute.MemberName);
                var hasLineNumber = specialAttr.GetByNamed<bool>(Consts.ActivitySpecialValueAttribute.LineNumber);
                var specialCodes = new List<string>(3);
                if (hasFilePath)
                {
                    specialCodes.Add("tags.Add(\"special.filepath\",filePath);");
                }
                if (hasMemberName)
                {
                    specialCodes.Add("tags.Add(\"special.membername\",memberName);");
                }
                if (hasLineNumber)
                {
                    specialCodes.Add("tags.Add(\"special.linenumber\",lineNumber);");
                }
                if (specialCodes.Count != 0)
                {
                    specialTagCodes = $@"
if(tags == null)
{{
    tags = new global::System.Diagnostics.ActivityTagsCollection();
}}
{string.Join("\n", specialCodes)}
";
                }
            }

            var setStatusCode = string.Empty;
            if (setStatus != null)
            {
                var head = "global::System.Diagnostics.ActivityStatusCode.";
                switch (setStatus.Value)
                {
                    case 1:head += "Ok"; break;
                    case 2:head += "Error";break;
                    default:
                        head += "Unset"; break;
                }
                var description = "null";
                if (setStatusAttr?.GetByNamed<bool?>(Consts.ActivityStatusAttribute.WithDescript) ?? false)
                {
                    //Has exception?
                    var exceptionPar = method.Parameters.FirstOrDefault(x => EventSourceHelper.IsExceptionType(model, x.Type));
                    if (exceptionPar!=null)
                    {
                        description = $"{exceptionPar.Name}?.Message";
                    }
                }
                setStatusCode = $"activity.SetStatus({head},{description});";
            }
            return $@"
#region {method.Name}
{activityMapToEventAttr}
{visibility} {staticKeyword} {method.ReturnType} {method.Name}({endsArgs})
{{
    {method.Name}(global::System.Diagnostics.Activity.Current,{invokeCodesNoConvert});
}}
{activityMapToEventAttr}
{visibility} {staticKeyword} {method.ReturnType} {method.Name}({argCode})
{{
    {beginCallEventSourceCodes}
    if(activity != null)
    {{
        global::System.Diagnostics.ActivityTagsCollection? tags = null;
        {eventTagCodes}
        {specialTagCodes}
        activity.AddEvent(new global::System.Diagnostics.ActivityEvent(""{method.Name}"", timestamp, tags));
        {setStatusCode}
        {tagCodes}
        On{method.Name}(activity,{invokeArgsJoined}timestamp, additionTags);
    }}
    {endCallEventSourceCodes}

}}
{staticKeyword} partial {method.ReturnType} On{method.Name}({argCode});


{withLogCode}
#endregion

";
        }

    }
}
