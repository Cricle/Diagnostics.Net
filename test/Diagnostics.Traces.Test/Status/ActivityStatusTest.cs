using Diagnostics.Generator.Core.Test;
using Diagnostics.Traces.Status;
using System.Diagnostics;

namespace Diagnostics.Traces.Test.Status
{
    [TestClass]
    public class ActivityStatusTest
    {
        [TestMethod]
        public void GivenNullFunction_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ActivityStatus((Func<ActivitySource, bool>)null!));
        }

        [TestMethod]
        public void ActivityStarted_StartActivityCount_MustAddOne()
        {
            var source = new ActivitySource("test");
            using var listner = new ActivityListenerBox("test");
            using (var status = new ActivityStatus(source))
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                Assert.AreEqual(status.StartActivityCount, 1);
                Assert.AreEqual(status.TotalActivityCount, 1);
                Assert.AreEqual(status.ErrorActivityCount, 0);
            }
        }

        [TestMethod]
        public void ActivityStarted_StartAndStopActivityCount_MustAllAddOne()
        {
            var source = new ActivitySource("test");
            using var listner = new ActivityListenerBox("test");
            using (var status = new ActivityStatus(source))
            {
                using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
                {
                }
                Assert.AreEqual(status.StartActivityCount, 0);
                Assert.AreEqual(status.TotalActivityCount, 1);
                Assert.AreEqual(status.ErrorActivityCount, 0);
            }
        }

        [TestMethod]
        public void ActivityStarted_StartAndStopFailActivityCount_MustAllAddOne()
        {
            var source = new ActivitySource("test");
            using var listner = new ActivityListenerBox("test");
            using (var status = new ActivityStatus(source))
            {
                using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
                {
                    activity?.SetStatus(ActivityStatusCode.Error);
                }
                Assert.AreEqual(status.StartActivityCount, 0);
                Assert.AreEqual(status.TotalActivityCount, 1);
                Assert.AreEqual(status.ErrorActivityCount, 1);
            }
        }
    }
}
