﻿using Microsoft.CodeAnalysis;

namespace Diagnostics.Generator
{
    internal static class Messages
    {
        public const string Category = "Diagnostics.Generator";

        public static readonly DiagnosticDescriptor NotSupportType = new DiagnosticDescriptor("DG0001",
            "EventSourceGenerator",
            "Unsupport event source type", Category,
             DiagnosticSeverity.Error, true, helpLinkUri: "https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.tracing.eventsource.writeeventcore?view=net-8.0");

        public static readonly DiagnosticDescriptor RelatedActivityIdOnlyOne = new DiagnosticDescriptor("DG0002",
            "EventSourceGenerator",
            "Related activity id only one or zero", Category,
             DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor RelatedActivityIdMustGuid = new DiagnosticDescriptor("DG0003",
            "EventSourceGenerator",
            "Related activity id must guid", Category,
             DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor FieldMustReturnNumber = new DiagnosticDescriptor("DG0004",
            "EventSourceGenerator",
            "Field type must int, long, double, float, Enum, IncrementingEventCounter, EventCounter", Category,
             DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor EventCounterMustType = new DiagnosticDescriptor("DG0005",
            "EventSourceGenerator",
            "Counter type is IncrementingEventCounter, EventCounter, the field type must be that", Category,
             DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor PollingCounterMustInputRate = new DiagnosticDescriptor("DG0006",
            "EventSourceGenerator",
            "IncrementingPollingCounter, IncrementingEventCounter must input DisplayRateTimeScaleMs and more than zero", Category,
             DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor PollingCounterMustSimpleType = new DiagnosticDescriptor("DG0007",
            "EventSourceGenerator",
            "PollingCounter or IncrementingPollingCounter field type must int, long, double, float", Category,
             DiagnosticSeverity.Warning, true);
        public static readonly DiagnosticDescriptor NameNeedEndWithEventSource = new DiagnosticDescriptor("DG0008",
            "The event source name recommendation end with EventSource, such as ProcessEventSource",
            "The event source name recommendation end with EventSource, such as ProcessEventSource", Category,
             DiagnosticSeverity.Info, true);
        public static readonly DiagnosticDescriptor WriteEventBody = new DiagnosticDescriptor("DG0009",
            "Auto write event body",
            "Auto write event body", Category,
             DiagnosticSeverity.Info, true);
        public static readonly DiagnosticDescriptor ForProviderMustInput = new DiagnosticDescriptor("DG0010",
            "EventSourceGenerator",
            "When ForAnysProviders=true, must input ForProviders", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor TheIdHasSames = new DiagnosticDescriptor("DG0011",
            "EventSourceGenerator",
            "The id \"{0}\" has same", Category,
             DiagnosticSeverity.Warning, true);
        public static readonly DiagnosticDescriptor NoProvider = new DiagnosticDescriptor("DG0012",
            "EventSourceGenerator",
            "The WithCreator is true, but found nothing providers", Category,
             DiagnosticSeverity.Warning, true);
        public static readonly DiagnosticDescriptor LevelNotSpport = new DiagnosticDescriptor("DG0013",
            "EventSourceGenerator",
            "The EventLevel \"{0}\" is not support", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor FailToParseArguments = new DiagnosticDescriptor("DG0014",
            "EventSourceGenerator",
            "Fail to parse arguments \"{0}\", the Arguments must like interval=123,a1=2 or \"interval\"=\"123\",\"a1\"=\"2\"", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor AccesstorInstanceMustOnlyOne = new DiagnosticDescriptor("DG0015",
            "EventSourceGenerator",
            "The EventSourceAccesstorInstanceAttribute only can be zero or one", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor AccesstorInstanceDeclareError = new DiagnosticDescriptor("DG0016",
            "EventSourceGenerator",
            "The AccesstorInstance must static interval/public", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor NoAccesstorNoCallTogether = new DiagnosticDescriptor("DG0017",
            "EventSourceGenerator",
            "The event source \"{0}\" has no GenerateSingleton or tag EventSourceAccesstorInstanceAttribute field or property, can't set WithCalledTogetherExtensions = ture", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor MeterNotFound = new DiagnosticDescriptor("DG0018",
            "EventSourceGenerator",
            "The meter member \"{0}\" was not found, please check the type \"{1}\" has any this member", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor UnknowMeter = new DiagnosticDescriptor("DG0019",
            "EventSourceGenerator",
            "The meter member type \"{0}\" was unknow, now avaliable types is Counter, Histogram and UpDownCounter", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor MeterMethodInputError = new DiagnosticDescriptor("DG0020",
            "EventSourceGenerator",
            "The meter method first paramter must same as counter generic type \"{0}\"", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor MeterMethodError = new DiagnosticDescriptor("DG0021",
            "EventSourceGenerator",
            "The meter method must return void and not generic method", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor TagAsTargetNotClassOrStructError = new DiagnosticDescriptor("DG0022",
            "EventSourceGenerator",
            "The \"{0}\" is not class or struct", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor TagAsLoopReferenceError = new DiagnosticDescriptor("DG0023",
            "EventSourceGenerator",
            "The \"{0}\" is loop reference, path is \"{1}\", the generator can't parse loop reference", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor TagAsSelfMustNotStaticError = new DiagnosticDescriptor("DG0024",
            "EventSourceGenerator",
            "Tag as self must not static", Category,
             DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor EventIdHasUsed = new DiagnosticDescriptor("DG0025",
            "EventSourceGenerator",
            "The special event id {0} has been used", Category,
             DiagnosticSeverity.Error, true);
    }
}
