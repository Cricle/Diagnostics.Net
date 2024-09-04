using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Generator.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal class ActivityListenerBox : IDisposable
    {
        public ActivityListenerBox(string name)
        {
            Listener = new ActivityListener
            {
                ActivityStarted = activity => currentActivity = activity,
                ShouldListenTo = s => s.Name == name,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
            };
            ActivitySource.AddActivityListener(Listener);
        }

        [ThreadStatic]
        private static Activity? currentActivity;

        public static Activity? CurrentActivity => currentActivity;

        public ActivityListener Listener { get; }

        public void Dispose()
        {
            Listener.Dispose();
            currentActivity = null;
        }
    }
    [TestClass]
    public class ActivityAddEventEasyExtensionsTest
    {
        [TestMethod]
        public void StartActivity()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using var activity = ActivityAddEventEasyExtensions.StartActivity(source, "test");
            Assert.IsNotNull(activity);
        }

        [TestMethod]
        public void AddEvent_Plan()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                Assert.IsNotNull(activity);

                var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

                ActivityAddEventEasyExtensions.AddEvent(activity, "test", new ActivityTagsCollection(new KeyValuePair<string, object?>[]
                {
                    new KeyValuePair<string, object?>("a1",1)
                }), offset);
                Assert.AreEqual(activity!.Events.Count(), 1);
                Assert.AreEqual(activity!.Events.First().Name, "test");
                Assert.AreEqual(activity!.Events.First().Tags.First().Key, "a1");
                Assert.AreEqual(activity!.Events.First().Tags.First().Value, 1);
                Assert.AreEqual(activity!.Events.First().Timestamp, offset);
            }
        }

        [TestMethod]
        public void AddEvent_Plan_NullActivity()
        {
            Activity? activity = null;

            ActivityAddEventEasyExtensions.AddEvent(activity, "test", new ActivityTagsCollection(new KeyValuePair<string, object?>[]
            {
                new KeyValuePair<string, object?>("a1",1)
            }), default);
        }

        [TestMethod]
        public void AddEvent_Plan2()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                Assert.IsNotNull(activity);

                var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

                ActivityAddEventEasyExtensions.AddEvent(activity, "test", new Dictionary<string, object?>
                {
                    ["a1"] = 1
                }, offset);
                Assert.AreEqual(activity!.Events.Count(), 1);
                Assert.AreEqual(activity!.Events.First().Name, "test");
                Assert.AreEqual(activity!.Events.First().Tags.First().Key, "a1");
                Assert.AreEqual(activity!.Events.First().Tags.First().Value, 1);
                Assert.AreEqual(activity!.Events.First().Timestamp, offset);
            }
        }

        [TestMethod]
        public void AddEvent_Plan2_Null()
        {
            Activity? activity = null;

            var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

            ActivityAddEventEasyExtensions.AddEvent(activity, "test", new Dictionary<string, object?>
            {
                ["a1"] = 1
            }, offset);
        }

        [TestMethod]
        public void AddEvent_Plan3()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                Assert.IsNotNull(activity);

                var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

                ActivityAddEventEasyExtensions.AddEvent(activity, "test", new KeyValuePair<string, object?>[]
                {
                    new KeyValuePair<string, object?>("a1",1)
                }, offset);
                Assert.AreEqual(activity!.Events.Count(), 1);
                Assert.AreEqual(activity!.Events.First().Name, "test");
                Assert.AreEqual(activity!.Events.First().Tags.First().Key, "a1");
                Assert.AreEqual(activity!.Events.First().Tags.First().Value, 1);
                Assert.AreEqual(activity!.Events.First().Timestamp, offset);
            }
        }

        [TestMethod]
        public void AddEvent_Plan3_Null()
        {
            Activity? activity = null;

            var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

            ActivityAddEventEasyExtensions.AddEvent(activity, "test", new KeyValuePair<string, object?>[]
            {
                    new KeyValuePair<string, object?>("a1",1)
            }, offset);
        }

        [TestMethod]
        public void AddEvent_Plan4()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                Assert.IsNotNull(activity);

                var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

                ActivityAddEventEasyExtensions.AddEvent(activity, "test", ("a1", 1));
                Assert.AreEqual(activity!.Events.Count(), 1);
                Assert.AreEqual(activity!.Events.First().Name, "test");
                Assert.AreEqual(activity!.Events.First().Tags.First().Key, "a1");
                Assert.AreEqual(activity!.Events.First().Tags.First().Value, 1);
            }
        }

        [TestMethod]
        public void AddEvent_Plan4_Null()
        {
            Activity? activity = null;

            var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

            ActivityAddEventEasyExtensions.AddEvent(activity, "test", new KeyValuePair<string, object?>[]
            {
                    new KeyValuePair<string, object?>("a1",1)
            });
        }

        [TestMethod]
        public void AddEvent_Plan5()
        {
            var source = new ActivitySource("test");
            using var listener = new ActivityListenerBox("test");
            using (var activity = source.StartActivity("test", ActivityKind.Internal, default(ActivityContext)))
            {
                Assert.IsNotNull(activity);

                var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

                ActivityAddEventEasyExtensions.AddEvent(activity, "test", new (string, object?)[] { ("a1", 1) }, offset);
                Assert.AreEqual(activity!.Events.Count(), 1);
                Assert.AreEqual(activity!.Events.First().Name, "test");
                Assert.AreEqual(activity!.Events.First().Tags.First().Key, "a1");
                Assert.AreEqual(activity!.Events.First().Tags.First().Value, 1);
                Assert.AreEqual(activity!.Events.First().Timestamp, offset);
            }
        }

        [TestMethod]
        public void AddEvent_Plan5_Null()
        {
            Activity? activity = null;

            var offset = DateTimeOffset.Parse("2024-09-04 10:08:00+0");

            ActivityAddEventEasyExtensions.AddEvent(activity, "test", new (string, object?)[] { ("a1", 1) }, offset);
        }
    }
}
