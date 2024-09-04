using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    public record struct ActivityEventEntity
    {
        public string? Name { get; set; }

        public DateTime Timestamp { get; set; }

        public Dictionary<string, string?>? Tags { get; set; }
    }
}
