using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    public record struct ActivityLinkEntity
    {
        public ActivityLinkContextEntity Context { get; set; }

        public Dictionary<string, string?>? Tags { get; set; }
    }
}
