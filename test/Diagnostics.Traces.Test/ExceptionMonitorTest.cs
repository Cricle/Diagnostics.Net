using Diagnostics.Generator.Core;
using Diagnostics.Generator.Core.Test;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class ExceptionMonitorTest
    {
        [ExcludeFromCodeCoverage]
        class BatchOperatorHandler : IBatchOperatorHandler<TraceExceptionInfo>
        {
            public TraceExceptionInfo[]? Infos;

            public Task HandleAsync(BatchData<TraceExceptionInfo> inputs, CancellationToken token)
            {
                Infos = inputs.ToArray();
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
        public void GivenNullInit_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExceptionMonitor(null));
        }

        [TestMethod]
        public void ExceptionRaise_MustCaptched()
        {
            var ex = new InvalidOperationException("test");
            var handler = new BatchOperatorHandler();
            using var monitor = new ExceptionMonitor(handler, 1) { CatchMode = ExceptionCatchMode.Full };
            Assert.AreEqual(monitor.CatchMode, ExceptionCatchMode.Full);
            try
            {
                throw ex;
            }
            catch (Exception)
            {
            }

            WaitAllComplated(monitor.exceptionOperator);

            Assert.IsNotNull(handler.Infos);
            Assert.AreEqual(handler.Infos.Length, 1);
            Assert.IsInstanceOfType<InvalidOperationException>(handler.Infos[0].Exception);
        }

        [TestMethod]
        public void ExceptionRaise_OnlyHasActivity_NothingCaptched()
        {
            var ex = new InvalidOperationException("test");
            var handler = new BatchOperatorHandler();
            using var monitor = new ExceptionMonitor(handler, 1) { CatchMode = ExceptionCatchMode.OnlyHasActivity };
            Assert.AreEqual(monitor.CatchMode, ExceptionCatchMode.OnlyHasActivity);
            try
            {
                throw ex;
            }
            catch (Exception)
            {
            }

            WaitAllComplated(monitor.exceptionOperator);

            Assert.IsNull(handler.Infos);
        }
        [TestMethod]
        public void ExceptionRaise_OnlyHasActivity_HasActivity()
        {
            using var listener = new ActivityListenerBox("test");

            var source = new ActivitySource("test");
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                var ex = new InvalidOperationException("test");
                var handler = new BatchOperatorHandler();
                using var monitor = new ExceptionMonitor(handler, 1) { CatchMode = ExceptionCatchMode.OnlyHasActivity };
                try
                {
                    throw ex;
                }
                catch (Exception)
                {
                }

                WaitAllComplated(monitor.exceptionOperator);

                Assert.IsNotNull(handler.Infos);
                Assert.AreEqual(handler.Infos.Length, 1);
                Assert.AreEqual(handler.Infos[0].Exception, ex);
            }

        }
    }
}
