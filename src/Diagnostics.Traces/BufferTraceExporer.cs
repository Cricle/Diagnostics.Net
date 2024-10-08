﻿using Diagnostics.Generator.Core;
using OpenTelemetry;

namespace Diagnostics.Traces
{
    public class BufferTraceExporer<T> : BaseExporter<T>
       where T : class
    {
        protected readonly BufferOperator<T> bufferOperator;

        public BufferTraceExporer(IOperatorHandler<T> handler)
            : this(new BufferOperator<T>(handler))
        {

        }
        public BufferTraceExporer(BufferOperator<T> bufferOperator)
        {
            this.bufferOperator = bufferOperator ?? throw new ArgumentNullException(nameof(bufferOperator));
        }

        public override ExportResult Export(in Batch<T> batch)
        {
            foreach (var item in batch)
            {
                bufferOperator.Add(item);
            }
            return ExportResult.Success;
        }

        protected override void Dispose(bool disposing)
        {
            bufferOperator.Dispose();
        }
    }
}
