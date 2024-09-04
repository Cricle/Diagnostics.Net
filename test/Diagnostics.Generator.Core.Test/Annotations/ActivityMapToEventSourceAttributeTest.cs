using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class ActivityMapToEventSourceAttributeTest
    {
        [TestMethod]
        public void Given_MustEquals()
        {
            var eventType = typeof(object);
            var mappedCount = 1;

            var attr = new ActivityMapToEventSourceAttribute(eventType, mappedCount);

            Assert.AreEqual(attr.EventType, eventType);
            Assert.AreEqual(attr.MappedCount, mappedCount);
        }
    }
}
