using System.Diagnostics;
using System.Text.Json;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class ActivityJsonConverterTests
    {
        [TestMethod]
        public void Write_ShouldSerializeActivityCorrectly()
        {
            // Arrange
            var activity = new Activity("TestActivity")
                .SetIdFormat(ActivityIdFormat.W3C)
                .SetParentId("parent-id")
                .Start();

            activity.SetStatus(ActivityStatusCode.Ok, "Test status description");
            activity.DisplayName = "Test Activity Display Name";
            activity.SetTag("tag1", "value1");
            activity.SetTag("tag2", "value2");
            activity.AddEvent(new ActivityEvent("TestEvent", DateTime.UtcNow, new ActivityTagsCollection(new KeyValuePair<string, object?>[]
            {
                new KeyValuePair<string, object?>("event-tag1", "event-value1")
            })));
            activity.AddBaggage("baggageKey1", "baggageValue1");
            activity.AddBaggage("baggageKey2", "baggageValue2");

            var options = new JsonSerializerOptions
            {
                Converters = { ActivityJsonConverter.Instance },
                WriteIndented = true
            };

            // Act
            string json = JsonSerializer.Serialize(activity, options);

            // Assert
            Assert.IsNotNull(json);
            StringAssert.Contains(json, "\"Id\": \"" + activity.Id + "\"");
            StringAssert.Contains(json, "\"Status\": \"Ok\"");
            StringAssert.Contains(json, "\"DisplayName\": \"Test Activity Display Name\"");
            StringAssert.Contains(json, "\"ParentId\": \"parent-id\"");
            StringAssert.Contains(json, "\"tag1\": \"value1\"");
            StringAssert.Contains(json, "\"tag2\": \"value2\"");
            StringAssert.Contains(json, "\"baggageKey1\": \"baggageValue1\"");
            StringAssert.Contains(json, "\"baggageKey2\": \"baggageValue2\"");
            StringAssert.Contains(json, "\"IsStopped\": false");  // activity hasn't been stopped yet
        }

        [TestMethod]
        public void Write_ShouldHandleEmptyActivity()
        {
            // Arrange
            var activity = new Activity("EmptyActivity").Start();

            var options = new JsonSerializerOptions
            {
                Converters = { ActivityJsonConverter.Instance },
                WriteIndented = true
            };

            // Act
            string json = JsonSerializer.Serialize(activity, options);

            // Assert
            Assert.IsNotNull(json);
            StringAssert.Contains(json, "\"Id\": \"" + activity.Id + "\"");
            StringAssert.Contains(json, "\"DisplayName\": \"EmptyActivity\"");
        }

        [TestMethod]
        public void Write_ShouldSerializeActivityWithEventsAndLinks()
        {
            // Arrange
            var activity = new Activity("ActivityWithEventsAndLinks")
                .SetIdFormat(ActivityIdFormat.W3C)
                .Start();

            activity.AddEvent(new ActivityEvent("Event1", DateTime.UtcNow));
            activity.AddEvent(new ActivityEvent("Event2", DateTime.UtcNow.AddSeconds(10)));

            activity.AddTag("Key", "Value");

            var options = new JsonSerializerOptions
            {
                Converters = { ActivityJsonConverter.Instance },
                WriteIndented = true
            };

            // Act
            string json = JsonSerializer.Serialize(activity, options);

            // Assert
            Assert.IsNotNull(json);
            StringAssert.Contains(json, "\"Name\": \"Event1\"");
            StringAssert.Contains(json, "\"Name\": \"Event2\"");
        }
    }
}
