using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class ActivityMapToEventAttributeTest
    {
        [TestMethod]
        public void Given_MustEuqlas()
        {
            var eventId = 1;
            var methodName = "m0";
            var paramterTypes = new Type[] { typeof(object) };

            var attr = new ActivityMapToEventAttribute(eventId, methodName, paramterTypes);

            Assert.AreEqual(attr.EventId, eventId);
            Assert.AreEqual(attr.MethodName, methodName);
            Assert.AreEqual(attr.ParamterTypes, paramterTypes);
        }
    }
}
