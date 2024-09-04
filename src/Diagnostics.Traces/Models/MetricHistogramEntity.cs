using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    public record struct MetricHistogramEntity
    {
        public double RangeLeft { get; set; }

        public double RangeRight { get; set; }

        public int BucketCount { get; set; }
    }
}
