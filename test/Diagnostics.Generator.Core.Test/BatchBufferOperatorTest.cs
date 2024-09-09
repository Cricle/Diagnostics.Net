using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Generator.Core.Test
{
    [TestClass]
    public class BatchBufferOperatorTest
    {
        [ExcludeFromCodeCoverage]
        class ExceptionBatchOperatorHandler<T> : IBatchOperatorHandler<T>
        {
            public Task HandleAsync(BatchData<T> inputs, CancellationToken token)
            {
                throw new ArgumentException();
            }
        }

        [ExcludeFromCodeCoverage]
        class EmptyBatchOperatorHandler<T> : IBatchOperatorHandler<T>
        {
            public Task HandleAsync(BatchData<T> inputs, CancellationToken token)
            {
                return Task.CompletedTask;
            }
        }

        [ExcludeFromCodeCoverage]
        class CopyBatchOperatorHandler<T> : IBatchOperatorHandler<T>
        {
            public T[]? Datas;

            public Task HandleAsync(BatchData<T> inputs, CancellationToken token)
            {
                Datas = inputs.Datas.ToArray();
                return Task.CompletedTask;
            }
        }
        [ExcludeFromCodeCoverage]
        class CopyAllBatchOperatorHandler<T> : IBatchOperatorHandler<T>
        {
            public List<T[]> Datas { get; } = new List<T[]>();

            public Task HandleAsync(BatchData<T> inputs, CancellationToken token)
            {
                Datas.Add(inputs.Datas.ToArray());
                return Task.CompletedTask;
            }
        }

        private void WaitAllComplated<T>(BatchBufferOperator<T> @operator)
        {
            if (!SpinWait.SpinUntil(() => @operator.UnComplatedCount == 0 && @operator.BufferIndex == 0, TimeSpan.FromSeconds(10)))
            {
                Assert.Fail("The wait all complated is out of 10 seconds");
            }
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void Given_NullHandler_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BatchBufferOperator<object>(null));
        }

        [TestMethod]
        public void Given_MustEquals()
        {
            var handler = new EmptyBatchOperatorHandler<int>();
            var bufferSize = 111;
            var swapDelayTimeMs = 222;

            using var @operator = new BatchBufferOperator<int>(handler, bufferSize: bufferSize, swapDelayTimeMs: swapDelayTimeMs);

            Assert.IsNotNull(@operator.Task);
            Assert.IsNotNull(@operator.TaskTimeLoop);

            Assert.AreEqual(@operator.Handler, handler);
            Assert.AreEqual(@operator.BufferSize, bufferSize);
            Assert.AreEqual(@operator.SwapDelayTimeMs, swapDelayTimeMs);
        }

        [TestMethod]
        public void Givens_ProcessMustGot()
        {
            var handler = new CopyBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 100, 100000);
            for (int i = 0; i < 100; i++)
            {
                @operator.Add(i);
            }

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsNotNull(handler.Datas);
            Assert.AreEqual(handler.Datas.Length, 100);
            Assert.IsTrue(handler.Datas.SequenceEqual(Enumerable.Range(0, 100)));

        }

        [TestMethod]
        public void Givens_TimeOut_ProcessMustGot()
        {
            var handler = new CopyBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 100000, 1000);
            for (int i = 0; i < 100; i++)
            {
                @operator.Add(i);
            }

            WaitAllComplated(@operator);
            Thread.Sleep(500);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsNotNull(handler.Datas);
            Assert.AreEqual(handler.Datas.Length, 100);
            Assert.IsTrue(handler.Datas.SequenceEqual(Enumerable.Range(0, 100)));

        }
        [TestMethod]
        public void Givens_AddRange_TimeOut_ProcessCapacityMustGot()
        {
            var handler = new CopyBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 100, 100000);
            @operator.AddRange(Enumerable.Range(0, 100).ToArray());

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsNotNull(handler.Datas);
            Assert.AreEqual(handler.Datas.Length, 100);
            Assert.IsTrue(handler.Datas.SequenceEqual(Enumerable.Range(0, 100)));

        }
        [TestMethod]
        public void Givens_AddRangeList_TimeOut_ProcessCapacityMustGot()
        {
            var handler = new CopyBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 100, 100000);
            @operator.AddRange(Enumerable.Range(0, 100).ToList());

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsNotNull(handler.Datas);
            Assert.AreEqual(handler.Datas.Length, 100);
            Assert.IsTrue(handler.Datas.SequenceEqual(Enumerable.Range(0, 100)));

        }
        [TestMethod]
        public void Givens_AddRangeConcurrentList_TimeOut_ProcessCapacityMustGot()
        {
            var handler = new CopyBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 100, 100000);
            @operator.AddRange(new LinkedList<int>(Enumerable.Range(0, 100)));

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsNotNull(handler.Datas);
            Assert.AreEqual(handler.Datas.Length, 100);
            Assert.IsTrue(handler.Datas.SequenceEqual(Enumerable.Range(0, 100)));

        }
        [TestMethod]
        public void Givens_AddRange_OutCapacity_ProcessCapacityMustGot()
        {
            var handler = new CopyAllBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 50, 1000);
            @operator.AddRange(Enumerable.Range(0, 55).ToArray());

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);
            if (!SpinWait.SpinUntil(() => handler.Datas.Count == 2, TimeSpan.FromSeconds(10)))
            {
                Assert.Fail($"Wait handler.Datas.Count == 2 timeout 10 seconds");
            }
            Assert.AreEqual(handler.Datas.Count, 2);
            Assert.AreEqual(handler.Datas[0].Length, 50);
            Assert.AreEqual(handler.Datas[1].Length, 5);
            Assert.IsTrue(handler.Datas[0].SequenceEqual(Enumerable.Range(0, 50)));
            Assert.IsTrue(handler.Datas[1].SequenceEqual(Enumerable.Range(50, 5)));

        }


        [TestMethod]
        public void Givens_AddRange_TimeOut_ProcessTimeOutMustGot()
        {
            var handler = new CopyBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 100000, 1000);
            @operator.AddRange(Enumerable.Range(0, 100).ToArray());

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsNotNull(handler.Datas);
            Assert.AreEqual(handler.Datas.Length, 100);
            Assert.IsTrue(handler.Datas.SequenceEqual(Enumerable.Range(0, 100)));

        }

        [TestMethod]
        public void ExceptionRaised_Event()
        {
            var handler = new ExceptionBatchOperatorHandler<int>();

            using var @operator = new BatchBufferOperator<int>(handler, 1, 1000);

            BatchOperatorExceptionEventArgs<int>? args = default;

            @operator.ExceptionRaised += (o, e) =>
            {
                Assert.AreEqual(o, @operator);
                args = e;
            };
            @operator.Add(1);

            WaitAllComplated(@operator);
            Assert.AreEqual(@operator.UnComplatedCount, 0);

            Assert.IsTrue(args.HasValue);
            Assert.AreEqual(args.Value.Inputs.Count, 1);
            Assert.AreEqual(args.Value.Inputs.Datas[0], 1);
            Assert.IsInstanceOfType<ArgumentException>(args.Value.Exception);
        }
    }
}
