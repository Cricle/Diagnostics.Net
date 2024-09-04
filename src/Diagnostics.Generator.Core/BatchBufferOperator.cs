using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Diagnostics.Generator.Core
{
    public class BatchBufferOperator<T> : IDisposable
    {
        private int disposedCount;
        private readonly Channel<BatchData<T>> channel;
        private readonly Task task, taskTimeLoop;
        private readonly CancellationTokenSource tokenSource;
        private readonly object locker;
        private T[] currentBuffer = null!;
        private int bufferIndex;

        public BatchBufferOperator(IBatchOperatorHandler<T> handler, int bufferSize = 512, int swapDelayTimeMs = 5000)
        {
            BufferSize = bufferSize;
            tokenSource = new CancellationTokenSource();
            locker = new object();
            channel = Channel.CreateUnbounded<BatchData<T>>();
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            task = Task.Factory.StartNew(HandleAsync, this, TaskCreationOptions.LongRunning);
            taskTimeLoop = Task.Factory.StartNew(HandleTimeLoopAsync, this, TaskCreationOptions.LongRunning);
            SwapDelayTimeMs = swapDelayTimeMs;
            Swap();
        }

        public Task Task => task;

        public Task TaskTimeLoop => taskTimeLoop;

        public int BufferSize { get; }

        public int BufferIndex => bufferIndex;

        public int SwapDelayTimeMs { get; }

        public IBatchOperatorHandler<T> Handler { get; }

        public int UnComplatedCount => channel.Reader.Count;

        public event EventHandler<BatchOperatorExceptionEventArgs<T>>? ExceptionRaised;

        private async Task HandleTimeLoopAsync(object? state)
        {
            var opetator = (BatchBufferOperator<T>)state!;
            var tk = opetator.tokenSource;
            var delayTime = opetator.SwapDelayTimeMs;

            while (!tk.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(delayTime, tk.Token);
                    Swap();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    ExceptionRaised?.Invoke(this, new BatchOperatorExceptionEventArgs<T>(default, ex));
                }
            }
        }

        private async Task HandleAsync(object? state)
        {
            var opetator = (BatchBufferOperator<T>)state!;
            var tk = opetator.tokenSource;
            var reader = opetator.channel.Reader;
            var handler = opetator.Handler;

            BatchData<T> datas = default;

            while (!tk.IsCancellationRequested)
            {
                try
                {
                    datas = await reader.ReadAsync(tk.Token);
                    await handler.HandleAsync(datas, tk.Token);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    ExceptionRaised?.Invoke(this, new BatchOperatorExceptionEventArgs<T>(datas,ex));
                }
                finally
                {
                    datas.Dispose();
                    datas = default;
                }
            }
            tk.Dispose();
        }
        public void Add(T t)
        {
            lock (locker)
            {
                UnsafeAdd(t);
            }
        }
        public void AddRange(IReadOnlyCollection<T> values)
        {
            lock (locker)
            {
                if (BufferSize - BufferIndex >= values.Count)
                {
                    CopyTo(currentBuffer.AsSpan(bufferIndex), values, 0);
                    SwapIfNeeds();
                }
                else
                {
                    foreach (var item in values)
                    {
                        UnsafeAdd(item);
                    }
                }
            }
        }
        private void UnsafeAdd(T t)
        {
            currentBuffer[bufferIndex++] = t;
            SwapIfNeeds();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SwapIfNeeds()
        {
            if (bufferIndex >= BufferSize)
            {
                Swap();
            }
        }
        private void CopyTo(Span<T> buffer, IReadOnlyCollection<T> ts, int startIndex)
        {
            if (ts is T[] array)
            {
                array.AsSpan(startIndex).CopyTo(buffer);
                bufferIndex += ts.Count;
            }
#if NET8_0_OR_GREATER
            else if (ts is List<T> list)
            {
                CollectionsMarshal.AsSpan(list).Slice(startIndex).CopyTo(buffer);
                bufferIndex += ts.Count;
            }
#endif
            else
            {
                foreach (var item in ts.Skip(startIndex))
                {
                    currentBuffer[bufferIndex++] = item;
                }
            }
        }
        private void Swap()
        {
            if (bufferIndex == 0 && currentBuffer != null)
            {
                return;
            }
            if (currentBuffer != null)
            {
                channel.Writer.WriteAsync(new BatchData<T>(currentBuffer, bufferIndex)).GetAwaiter().GetResult();
            }
            currentBuffer = ArrayPool<T>.Shared.Rent(BufferSize);
            bufferIndex = 0;
        }

        public void Dispose()
        {
            if (Interlocked.Increment(ref disposedCount) > 1)
            {
                return;
            }
            channel.Writer.Complete();
            tokenSource.Cancel();
        }
    }
}
