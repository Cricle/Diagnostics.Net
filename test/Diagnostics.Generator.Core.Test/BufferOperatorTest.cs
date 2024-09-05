using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Diagnostics.Generator.Core.Test
{
    [TestClass]
    public class BufferOperatorTest
    {
        [ExcludeFromCodeCoverage]
        class ExceptionOpetatorHandler<T> : IOperatorHandler<T>
        {
            public Task HandleAsync(T input, CancellationToken token)
            {
                throw new ArgumentException();
            }
        }
        [ExcludeFromCodeCoverage]
        class OpetatorHandler<T> : IOperatorHandler<T>
        {
            public T? LastValue;

            public Task HandleAsync(T input, CancellationToken token)
            {
                LastValue = input;
                return Task.CompletedTask;
            }
        }
        [ExcludeFromCodeCoverage]
        class StoreOpetatorHandler<T> : IOperatorHandler<T>
        {
            public List<T> LastValues { get; } = new List<T>();

            public async Task HandleAsync(T input, CancellationToken token)
            {
                LastValues.Add(input);
                await Task.Delay(1, token);
                await Task.Yield();
            }
        }
        [ExcludeFromCodeCoverage]
        class AsyncOpetatorHandler<T> : IOperatorHandler<T>
        {
            public T? LastValue;

            public async Task HandleAsync(T input, CancellationToken token)
            {
                LastValue = input;
                await Task.Yield();
            }
        }

        [TestMethod]
        public void GivenNullHandler_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BufferOperator<object>(null));
        }

        [TestMethod]
        public void GivenMustEquals()
        {
            var handler = new OpetatorHandler<object>();
            var wait = true;
            var continueCaptureContext = true;

            using var @operator = new BufferOperator<object>(handler, wait, continueCaptureContext);

            Assert.IsNotNull(@operator.Task);
            Assert.IsNotNull(@operator.Reader);
            Assert.IsFalse(@operator.Task.IsCompleted);
            Assert.AreEqual(@operator.Handler, handler);
            Assert.AreEqual(@operator.Wait, wait);
            Assert.AreEqual(@operator.ContinueCaptureContext, continueCaptureContext);
        }

        [TestMethod]
        public void Disposed_TaskMust_Stop()
        {
            var handler = new OpetatorHandler<object>();

            var @operator = new BufferOperator<object>(handler);
            @operator.Dispose();

            if (!SpinWait.SpinUntil(() => @operator.Task.IsCompletedSuccessfully, TimeSpan.FromSeconds(10)))
            {
                Assert.Fail("The operator task stop timeout 10 seconds");
            }
        }

        private void WaitAllComplated<T>(BufferOperator<T> @operator)
        {
            if (!SpinWait.SpinUntil(() => @operator.UnComplatedCount == 0, TimeSpan.FromSeconds(10)))
            {
                Assert.Fail("The wait all complated is out of 10 seconds");
            }
        }

        [TestMethod]
        public void Input_MustBeProcess()
        {
            var handler = new OpetatorHandler<int>();

            using var @operator = new BufferOperator<int>(handler);
            @operator.Add(1);
            WaitAllComplated(@operator);

            Assert.AreEqual(handler.LastValue, 1);
        }

        [TestMethod]
        public void Input_MustBeProcess_Async()
        {
            var handler = new AsyncOpetatorHandler<int>();

            using var @operator = new BufferOperator<int>(handler);
            @operator.Add(1);
            WaitAllComplated(@operator);

            Assert.AreEqual(handler.LastValue, 1);
        }

        [TestMethod]
        public void Input_Wait_MustSequert()
        {
            var handler = new StoreOpetatorHandler<int>();

            using var @operator = new BufferOperator<int>(handler, wait: true, continueCaptureContext: false);
            for (int i = 0; i < 100; i++)
            {
                @operator.Add(i);
            }
            WaitAllComplated(@operator);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(handler.LastValues[i], i);
            }
        }

        [TestMethod]
        public void OperatorThrowException_MustRaisedEvent()
        {
            var handler = new ExceptionOpetatorHandler<int>();

            using var @operator = new BufferOperator<int>(handler);

            BufferOperatorExceptionEventArgs<int>? args = null;

            @operator.ExceptionRaised += (o, e) =>
            {
                Assert.AreEqual(@operator, o);
                args = e;
            };
            @operator.Add(1);
            WaitAllComplated(@operator);

            Assert.IsTrue(args.HasValue);
            Assert.AreEqual(args.Value.Input, 1);
            Assert.IsInstanceOfType<ArgumentException>(args.Value.Exception);
        }

        [TestMethod]
        public void DisposedMore()
        {
            var handler = new OpetatorHandler<int>();

            var @operator = new BufferOperator<int>(handler);
            for (int i = 0; i < 2; i++)
            {
                @operator.Dispose();
            }
        }
    }
}
