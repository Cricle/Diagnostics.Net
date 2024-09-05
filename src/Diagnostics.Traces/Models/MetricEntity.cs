using OpenTelemetry.Metrics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    [JsonSerializable(typeof(MetricEntity))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class MetricEntityJsonSerializerContext : JsonSerializerContext
    {

    }
    [ExcludeFromCodeCoverage]
    [JsonSerializable(typeof(MetricEntity))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public partial class MetricEntityIgnoreNullJsonSerializerContext : JsonSerializerContext
    {

    }
    public class MetricEntity
    {
        public string? Name { get; set; }

        public string? Unit { get; set; }

        public MetricType MetricType { get; set; }

        public AggregationTemporality Temporality { get; set; }

        public string? Description { get; set; }

        public string? MeterName { get; set; }

        public string? MeterVersion { get; set; }

        public Dictionary<string, string>? MeterTags { get; set; }

        public DateTime CreateTime { get; set; }

        public List<MetricPointEntity>? Points { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, MetricEntityJsonSerializerContext.Default.MetricType);
        }
    }
}
