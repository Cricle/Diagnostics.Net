using Diagnostics.Generator.Core;
using OpenTelemetry.Metrics;

namespace Diagnostics.Traces
{
    public interface IMetricTraceHandler : IOperatorHandler<Metric>,IInputHandlerSync<Metric>
    {
    }
    public interface IBatchMetricTraceHandler : IBatchInputHandler<Metric>, IBatchInputHandlerSync<Metric>
    {
    }
}
