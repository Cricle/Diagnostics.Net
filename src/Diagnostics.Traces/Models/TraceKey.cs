using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    public readonly record struct TraceKey : IEquatable<TraceKey>
    {
        public TraceKey(string? traceId, string? spanId)
        {
            TraceId = traceId;
            SpanId = spanId;
        }

        public string? TraceId { get; }

        public string? SpanId { get; }
    }
}
