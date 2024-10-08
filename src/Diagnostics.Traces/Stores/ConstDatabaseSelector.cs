﻿namespace Diagnostics.Traces.Stores
{
    public class ConstDatabaseSelector<TResult> : IUndefinedDatabaseSelector<TResult>
        where TResult : IDatabaseCreatedResult
    {
        public ConstDatabaseSelector(TResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public TResult Result { get; }

        public IList<IUndefinedDatabaseAfterSwitched<TResult>> AfterSwitcheds => throw new NotSupportedException();

        public IList<IUndefinedResultInitializer<TResult>> Initializers => throw new NotSupportedException();

        public void Dispose()
        {
            if (Result is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public bool Flush()
        {
            return false;
        }

        public void ReportInserted(int count)
        {
        }

        public void UnsafeReportInserted(int count)
        {
        }

        public void UnsafeUsingDatabaseResult(Action<TResult> @using)
        {
            @using(Result);
        }

        public void UnsafeUsingDatabaseResult<TState>(TState state, Action<TResult, TState> @using)
        {
            @using(Result, state);
        }

        public TReturn UnsafeUsingDatabaseResult<TReturn>(Func<TResult, TReturn> @using)
        {
            return @using(Result);
        }

        public TReturn UnsafeUsingDatabaseResult<TState, TReturn>(TState state, Func<TResult, TState, TReturn> @using)
        {
            return @using(Result, state);
        }

        public void UsingDatabaseResult(Action<TResult> @using)
        {
            @using(Result);
        }

        public void UsingDatabaseResult<TState>(TState state, Action<TResult, TState> @using)
        {
            @using(Result, state);
        }

        public TReturn UsingDatabaseResult<TReturn>(Func<TResult, TReturn> @using)
        {
            return @using(Result);
        }

        public TReturn UsingDatabaseResult<TState, TReturn>(TState state, Func<TResult, TState, TReturn> @using)
        {
            return @using(Result, state);
        }
    }
}
