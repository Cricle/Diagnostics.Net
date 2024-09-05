using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class TimerHandlerBaseTest
    {
        [ExcludeFromCodeCoverage]
        class TestTimerHandler : TimerHandlerBase
        {
            public TestTimerHandler(TimeSpan delayTime) : base(delayTime)
            {
            }

            public int HandlerCount;

            protected override Task Handle()
            {
                HandlerCount++;
                return Task.CompletedTask;
            }
        }

        [ExcludeFromCodeCoverage]
        class ExceptionTimerHandler : TimerHandlerBase
        {
            public ExceptionTimerHandler(TimeSpan delayTime) : base(delayTime)
            {
            }

            protected override Task Handle()
            {
                throw new InvalidOperationException();
            }
        }

        [TestMethod]
        public void GivenZeroDelayTime_MustThhrowArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TestTimerHandler(TimeSpan.Zero));
        }

        [TestMethod]
        public async Task HandlerAfterDelayTime()
        {
            using var handler = new TestTimerHandler(TimeSpan.FromDays(1));
            await Task.Delay(500);
            Assert.AreEqual(handler.HandlerCount, 1);
        }

        [TestMethod]
        public async Task HandlerAfterDelayTime_Two()
        {
            using var handler = new TestTimerHandler(TimeSpan.FromMilliseconds(500));
            await Task.Delay(600);
            Assert.AreEqual(handler.HandlerCount, 2);
        }

        [TestMethod]
        public async Task RaisedException_EventMustRaised()
        {
            using var handler = new ExceptionTimerHandler(TimeSpan.FromMilliseconds(500));

            Exception? exception = null;

            handler.ExceptionRaised += (o, e) =>
            {
                Assert.AreEqual(o, handler);
                exception = e;
            };

            await Task.Delay(500);

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType<InvalidOperationException>(exception);
        }

        [TestMethod]
        public async Task AsyncTimerHandler_MustRaisedAsync()
        {
            int raisedCount = 0;

            Func<Task> func = async () =>
            {
                raisedCount++;
                await Task.Yield();
            };

            using var handler = new AsyncTimerHandler(TimeSpan.FromMilliseconds(500), func);

            await Task.Delay(600);

            Assert.AreEqual(raisedCount, 2);
        }

        [TestMethod]
        public async Task TimerHandler_MustRaisedAsync()
        {
            int raisedCount = 0;

            Action func = () =>
            {
                raisedCount++;
            };

            using var handler = new TimerHandler(TimeSpan.FromMilliseconds(500), func);

            await Task.Delay(600);

            Assert.AreEqual(raisedCount, 2);
        }
    }
}
