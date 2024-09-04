using System;

namespace Diagnostics.Generator.Core
{
    public readonly struct BatchOperatorExceptionEventArgs<T>
    {
        public BatchOperatorExceptionEventArgs(in BatchData<T> inputs, Exception exception)
        {
            Inputs = inputs;
            Exception = exception;
        }

        public BatchData<T> Inputs { get; }

        public Exception Exception { get; }
    }
}
