using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class TraceHandlerBaseTest
    {
        [ExcludeFromCodeCoverage]
        class TestTraceHandler<TIdentity> : TraceHandlerBase<TIdentity>
            where TIdentity : IEquatable<TIdentity>
        {
            public bool Activity;
            public override void Handle(Activity input)
            {
                Activity = true;
            }

            public bool LogRecord;
            public override void Handle(LogRecord input)
            {
                LogRecord = true;
            }

            public bool Metric;
            public override void Handle(Metric input)
            {
                Metric = true;
            }

            public bool Activitys;
            public override void Handle(in Batch<Activity> inputs)
            {
                Activitys = true;
            }


            public bool LogRecords;
            public override void Handle(in Batch<LogRecord> inputs)
            {
                LogRecords = true;
            }

            public bool Metrics;
            public override void Handle(in Batch<Metric> inputs)
            {
                Metrics = true;
            }
        }

        [TestMethod]
        public async Task CancaledTokenInput_MustThrowOp()
        {
            using var source = new CancellationTokenSource();
            source.Cancel();
            var handler = new TestTraceHandler<int>();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => handler.HandleAsync(default(Activity), source.Token));
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => handler.HandleAsync(default(LogRecord), source.Token));
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => handler.HandleAsync(default(Metric), source.Token));
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => handler.HandleAsync(default(Batch<Activity>), source.Token));
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => handler.HandleAsync(default(Batch<LogRecord>), source.Token));
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => handler.HandleAsync(default(Batch<Metric>), source.Token));
        }

        [TestMethod]
        public async Task CallWillCan_SyncMethod()
        {
            var handler = new TestTraceHandler<int>();

            await handler.HandleAsync(default(Activity), default);
            Assert.IsTrue(handler.Activity);

            await handler.HandleAsync(default(LogRecord), default);
            Assert.IsTrue(handler.LogRecord);

            await handler.HandleAsync(default(Metric), default);
            Assert.IsTrue(handler.Metric);

            await handler.HandleAsync(default(Batch<Activity>), default);
            Assert.IsTrue(handler.Activitys);

            await handler.HandleAsync(default(Batch<LogRecord>), default);
            Assert.IsTrue(handler.LogRecords);

            await handler.HandleAsync(default(Batch<Metric>), default);
            Assert.IsTrue(handler.Metrics);
        }
    }
}
