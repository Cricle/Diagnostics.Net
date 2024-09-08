using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;

namespace Diagnostics.Generator.Core.Test
{
    [TestClass]
    public class EventSourceDiagnosticTest
    {
        [ExcludeFromCodeCoverage]
        [EventSource(Name = "TestEventSource")]
        class TestEventSource:EventSource
        {
            [Event(1,Level = EventLevel.Informational)]
            public void Raise()
            {
                WriteEvent(1);
            }
        }

        [ExcludeFromCodeCoverage]
        [EventSource(Name = "TestEventSource1")]
        class TestEventSource1 : EventSource
        {
            [Event(1, Level = EventLevel.Informational)]
            public void Raise()
            {
                WriteEvent(1);
            }
        }

        [TestMethod]
        public async Task Once()
        {
            var ev = new TestEventSource1();
            var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var tsk = Task.Factory.StartNew(() => EventSourceDiagnostic.GetOnceAsync(ev, token: timeoutToken.Token)).Unwrap();
            await Task.Delay(1000);
            ev.Raise();
            await tsk;
            Assert.AreEqual(tsk.Result.EventName, "Raise");
        }
        [TestMethod]
        public async Task Once_Cancel()
        {
            var ev = new TestEventSource();
            var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var tsk = Task.Factory.StartNew(() => EventSourceDiagnostic.GetOnceAsync(ev, token: timeoutToken.Token)).Unwrap();
            timeoutToken.Cancel();
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(async() => await tsk);
        }
    }
}
