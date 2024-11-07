using Diagnostics.Generator.Core.Annotations;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Dignostics.Aspire.ApiService.Internals
{
    [EventSourceGenerate(GenerateSingleton = true)]
    internal partial class ApiEventSource:EventSource
    {
        public const string EventName = "Dignostics.Api";

    }

    internal static partial class ApiActivity
    {
        public static readonly ActivitySource Source = new ActivitySource(ApiEventSource.EventName);
    }
    [MapToActivity(typeof(ApiActivity), WithEventSourceCall = true, GenerateWithLog = true)]
    [MapToEventSource(typeof(ApiEventSource))]
    internal partial class Logs
    {
        [Event(1)]
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Start select weatherforecast data")]
        public static partial void StartSelectWeatherforecast(ILogger logger);

        [Event(1)]
        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "End select weatherforecast data")]
        public static partial void EndSelectWeatherforecast(ILogger logger);
    }
}
