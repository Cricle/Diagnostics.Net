using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class CounterItemAttributeTest
    {
        [TestMethod]
        public void Given_MustEquals()
        {
            var eventName = "eventName";

            var attr = new CounterItemAttribute(eventName);

            Assert.AreEqual(attr.EventName, eventName);
        }
        [TestMethod]
        public void Given_NullName_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CounterItemAttribute(null));
        }
    }
}
