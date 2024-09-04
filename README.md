<h2 align="center">
Diagnostics.Net
</h2>
<div align="center">

[![.NET Build](https://github.com/Cricle/Diagnostics.Net/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Diagnostics.Net/actions/workflows/dotnet.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/c98ef772145b4d3d85614264858c65de)](https://app.codacy.com/gh/Cricle/Diagnostics.Net/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

</div>

<h6 align="center">
A fast, easily, diagnostics collector and diagnostics generation library for .NET
</h6>

#### What is this

It is a library for diagnostics gneration(use Roslyn), and a library for diagnostics collection. Like use to OTEL, or any others diagnostic captcher.

It can generate code only you add **some** attributes.

#### How to use

Install the [Fast.Diagnostics.Generator](https://www.nuget.org/packages/Fast.Diagnostics.Generator)

##### If you only want generated EventSource's events

Add `EventSourceGenerateAttribute` in your `EventSource` and make class `unsafe` and `partial`, add `partial` to event method if you want to auto generated.


<details>
<summary>Sample</summary>

```csharp
[EventSourceGenerate]//<--- Add this
internal unsafe partial class TestEventSource : EventSource
{
    [Event(1)]
    public partial void A(double dt);
}

//Will generated
#pragma warning disable CS8604
#nullable enable
namespace evev
{
    internal partial class TestEventSource
    {
        public partial void A(double dt)
        {
            global::System.Diagnostics.Tracing.EventSource.EventData* datas = stackalloc global::System.Diagnostics.Tracing.EventSource.EventData[1];
            datas[0] = new global::System.Diagnostics.Tracing.EventSource.EventData
            {
                DataPointer = (nint)(&dt),
                Size = sizeof(double)
            };
            WriteEventWithRelatedActivityIdCore(1, null, 1, datas);
            OnA(dt);
        }

        [global::System.Diagnostics.Tracing.NonEventAttribute]
        partial void OnA(double dt);
    }
}
#nullable restore
#pragma warning restore
```

</details>

##### If you want generated EventSource's events and ActivitySource's events, make them linkage

Any time we need to make eventsource and activitysource linkage, so you can add `MapToActivityAttribute` to generate activity event method.

<details>
<summary>Sample</summary>

```csharp
internal static partial class MyActivity
{
    private static readonly string? version = typeof(MyActivity).Assembly.GetName().Version?.ToString();//The version

    public static readonly ActivitySource source = new ActivitySource(MyEventSource.EventName, version);//The activitsource instance
}

[MapToActivity(typeof(MyActivity), WithEventSourceCall = true)]
[EventSource(Name = EventName), EventSourceGenerate(GenerateSingleton = true)]//Make the eventsource and activitysource linkage
internal unsafe partial class MyEventSource : EventSource
{
    public const string EventName = "My";

    [ActivityTag("projectId", "{0}")]//The tag for activity event
    [Event(1, Keywords = Keywords.Kinds)]
    public partial void Go(string projectId, string projectPath);

    static class Keywords
    {
        public const EventKeywords Kinds = (EventKeywords)1;
    }
}

//Will generated
#pragma warning disable CS8604
#nullable enable
namespace evev
{
    internal unsafe partial class MyEventSource
    {
        [Diagnostics.Generator.Core.Annotations.EventSourceAccesstorInstanceAttribute]
        public static readonly global::evev.MyEventSource Instance = new global::evev.MyEventSource();
        public partial void Go(string projectId, string projectPath)
        {
            global::System.Diagnostics.Tracing.EventSource.EventData* datas = stackalloc global::System.Diagnostics.Tracing.EventSource.EventData[2];
            datas[0] = new global::System.Diagnostics.Tracing.EventSource.EventData
            {
                DataPointer = projectId == null ? global::System.IntPtr.Zero : (nint)global::System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::System.Runtime.InteropServices.MemoryMarshal.GetReference(global::System.MemoryExtensions.AsSpan(projectId))),
                Size = projectId == null ? 0 : checked((projectId.Length + 1) * sizeof(char))
            };
            datas[1] = new global::System.Diagnostics.Tracing.EventSource.EventData
            {
                DataPointer = projectPath == null ? global::System.IntPtr.Zero : (nint)global::System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::System.Runtime.InteropServices.MemoryMarshal.GetReference(global::System.MemoryExtensions.AsSpan(projectPath))),
                Size = projectPath == null ? 0 : checked((projectPath.Length + 1) * sizeof(char))
            };
            WriteEventWithRelatedActivityIdCore(1, null, 2, datas);
            OnGo(projectId, projectPath);
        }

        [global::System.Diagnostics.Tracing.NonEventAttribute]
        partial void OnGo(string projectId, string projectPath);
    }
}
#nullable restore
#pragma warning restore

#pragma warning disable CS8604
#nullable enable
namespace evev
{
    [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventSourceAttribute(typeof(global::evev.MyEventSource), 1)]
    internal partial class MyActivity
    {
#region Go
        [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute(1, "Go", new global::System.Type[] { typeof(string), typeof(string) })]
        public static void Go(string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            Go(global::System.Diagnostics.Activity.Current, projectId, projectPath, timestamp, additionTags);
        }

        [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute(1, "Go", new global::System.Type[] { typeof(string), typeof(string) })]
        public static void Go(global::System.Diagnostics.Activity? activity, string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            if (global::evev.MyEventSource.Instance.IsEnabled())
            {
                global::evev.MyEventSource.Instance.Go(projectId, projectPath);
            }

            if (activity != null)
            {
                global::System.Diagnostics.ActivityTagsCollection? tags = null;
                tags = new global::System.Diagnostics.ActivityTagsCollection();
                tags["projectId"] = projectId;
                tags["projectPath"] = projectPath;
                if (additionTags != null)
                {
                    foreach (global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?> tag in additionTags)
                    {
                        if (tag.Key != null)
                        {
                            tags[tag.Key] = tag.Value;
                        }
                    }
                }

                activity.AddEvent(new global::System.Diagnostics.ActivityEvent("Go", timestamp, tags));
                activity.AddTag("projectId", global::System.String.Format("{0}", projectId, projectPath));
                OnGo(activity, projectId, projectPath, timestamp, additionTags);
            }
        }

        static partial void OnGo(global::System.Diagnostics.Activity? activity, string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0);
#endregion
    }
}
#nullable restore
#pragma warning restore

```

Useage

```csharp
using (var activity = MyActivity.source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
{
    MyActivity.Go("test", "testpath");//Will add activity event and eventsource event
}
```

</details>

##### If you want generated EventSource's events, ActivitySource's events and logs, make them linkage

Install the [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions), Add `MapToEventSourceAttribute` to logger class, and change `MapToActivityAttribute` to logger class, set `GenerateWithLog` = true

<details>
<summary>Sample</summary>

```csharp
internal static partial class MyActivity
{
    private static readonly string? version = typeof(MyActivity).Assembly.GetName().Version?.ToString();//The version

    public static readonly ActivitySource source = new ActivitySource(MyEventSource.EventName, version);//The activitsource instance
}

[EventSource(Name = EventName), EventSourceGenerate(GenerateSingleton = true)]
internal unsafe partial class MyEventSource : EventSource
{
    public const string EventName = "My";
}

[MapToEventSource(typeof(MyEventSource))]
[MapToActivity(typeof(MyActivity), WithEventSourceCall = true, GenerateWithLog = true)]
internal static partial class Logs
{
    [ActivityTag("projectId", "{0}")]//The tag for activity event
    [Event(1, Keywords = Keywords.Kinds)]
    [LoggerMessage(EventId = 1, Level = LogLevel.Information)]
    public static partial void Go(ILogger logger, string projectId, string projectPath);

    static class Keywords
    {
        public const EventKeywords Kinds = (EventKeywords)1;
    }
}

//Will generated

#pragma warning disable CS8604
#nullable enable
namespace evev
{
    [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventSourceAttribute(typeof(global::evev.MyEventSource), 1)]
    internal partial class MyActivity
    {
#region Go
        [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute(1, "Go", new global::System.Type[] { typeof(string), typeof(string) })]
        public static void Go(string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            Go(global::System.Diagnostics.Activity.Current, projectId, projectPath, timestamp, additionTags);
        }

        [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute(1, "Go", new global::System.Type[] { typeof(string), typeof(string) })]
        public static void Go(global::System.Diagnostics.Activity? activity, string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            if (global::evev.MyEventSource.Instance.IsEnabled())
            {
                global::evev.MyEventSource.Instance.Go(projectId, projectPath);
            }

            if (activity != null)
            {
                global::System.Diagnostics.ActivityTagsCollection? tags = null;
                tags = new global::System.Diagnostics.ActivityTagsCollection();
                tags["projectId"] = projectId;
                tags["projectPath"] = projectPath;
                if (additionTags != null)
                {
                    foreach (global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?> tag in additionTags)
                    {
                        if (tag.Key != null)
                        {
                            tags[tag.Key] = tag.Value;
                        }
                    }
                }

                activity.AddEvent(new global::System.Diagnostics.ActivityEvent("Go", timestamp, tags));
                activity.AddTag("projectId", global::System.String.Format("{0}", projectId, projectPath));
                OnGo(activity, projectId, projectPath, timestamp, additionTags);
            }
        }

        static partial void OnGo(global::System.Diagnostics.Activity? activity, string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0);
        [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute(1, "Go", new global::System.Type[] { typeof(string), typeof(string) })]
        public static void Go(global::Microsoft.Extensions.Logging.ILogger logger, string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            global::evev.Logs.Go(logger, projectId, projectPath);
            Go(global::System.Diagnostics.Activity.Current, projectId, projectPath, timestamp, additionTags);
        }

        [global::Diagnostics.Generator.Core.Annotations.ActivityMapToEventAttribute(1, "Go", new global::System.Type[] { typeof(string), typeof(string) })]
        public static void Go(global::Microsoft.Extensions.Logging.ILogger logger, global::System.Diagnostics.Activity? activity, string projectId, string projectPath, global::System.DateTimeOffset timestamp = default, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.Object?>>? additionTags = null, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string? filePath = null, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] string? memberName = null, [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            global::evev.Logs.Go(logger, projectId, projectPath);
            Go(activity, projectId, projectPath, timestamp, additionTags);
        }
#endregion
    }
}
#nullable restore
#pragma warning restore

//And more

```

Usaged

```csharp
ILoggerFactory loggerFactory = ...;

var logger = loggerFactory.CreateLogger<Program>();

using (var activity = MyActivity.source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
{
    MyActivity.Go(logger, "test", "testpath");//The call will send logger, activity event and eventsource event
}
```

</details>

#### Nexts

- At the repository

I wll add more test to ensure functional stability and simple the api

- At dotnet/runtime [#107069](https://github.com/dotnet/runtime/issues/107069)

I still don't know what the final implementation in dotnet/runtime will be like

#### External references

- [observability-with-otel](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)
- [logging](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/logging?tabs=command-line)
- [eventsource-getting-started](https://learn.microsoft.com/zh-cn/dotnet/core/diagnostics/eventsource-getting-started)
