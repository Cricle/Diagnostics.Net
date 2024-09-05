using Diagnostics.Generator.Core.Test;
using Diagnostics.Traces.Status;
using System.Diagnostics;

namespace Diagnostics.Traces.Test.Status
{
    [TestClass]
    public class SamplingActivityStatusTest
    {
        [TestMethod]
        public void ActivityStarted_BufferMustHasActivity()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var status = new SamplingActivityStatus(source))
            {
                using var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext));
                var statusList = status.Buffer;
                Assert.AreEqual(statusList.Count(), 1);
                Assert.AreEqual(statusList.First(), activity);
                Assert.IsTrue(status.Contains(activity!));
            }
        }

        [TestMethod]
        public void Start_And_Stop_TheActivityWillRemoved()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var status = new SamplingActivityStatus(source))
            {
                using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
                {

                }

                Assert.AreEqual(status.BufferSize, 0);
                Assert.IsFalse(status.Buffer.Any());
            }

        }
    }
}
