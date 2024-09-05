using Diagnostics.Generator.Core;
using OpenTelemetry.Logs;

namespace Diagnostics.Traces
{
    public interface ILogRecordTraceHandler : IOperatorHandler<LogRecord>,IInputHandlerSync<LogRecord>
    {
    }
    public interface IBatchLogRecordTraceHandler : IBatchInputHandler<LogRecord>, IBatchInputHandlerSync<LogRecord>
    {
    }
}
