using Diagnostics.Generator.Core;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class BufferTraceExporerTest
    {
        [ExcludeFromCodeCoverage]
        class OperatorHandler<T> : IOperatorHandler<T>
        {
            public List<T> Inputs { get; } = new List<T>();

            public Task HandleAsync(T input, CancellationToken token)
            {
                Inputs.Add(input);
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public void InitWithNull_WillThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BufferTraceExporer<object>((BufferOperator<object>)null));
        }

        //[TestMethod]
        //public async Task Export_AllDataRaised()
        //{
        //    var handler = new OperatorHandler<Activity>();
        //    var source = new ActivitySource("test");

        //    using var trace = Sdk.CreateTracerProviderBuilder()
        //        .AddSource("test")
        //        .AddProcessor(new BatchActivityExportProcessor(new BufferTraceExporer<Activity>(handler), scheduledDelayMilliseconds: 500))
        //        .Build();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        using (var activity = source.StartActivity("test" + i, ActivityKind.Internal, default(ActivityContext)))
        //        {

        //        }
        //    }

        //    await Task.Delay(900);

        //    Assert.AreEqual(10, handler.Inputs.Count);
        //    for (int i = 0; i < handler.Inputs.Count; i++)
        //    {
        //        Assert.AreEqual("test" + i, handler.Inputs[i].OperationName);
        //    }
        //}
    }
}
