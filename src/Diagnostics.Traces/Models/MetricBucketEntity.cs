using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    public record struct MetricBucketEntity
    {
        public double LowerBound { get; set; }

        public double UpperBound { get; set; }

        public int BucketCount { get; set; }
    }
}
