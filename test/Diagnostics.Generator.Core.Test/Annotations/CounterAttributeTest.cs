using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class CounterAttributeTest
    {
        [TestMethod]
        public void Given_MustEquals()
        {
            var name = "name";
            var type = CounterTypes.EventCounter;
            var displayRateTimeScaleMs = 1d;
            var displayName = "DisplayName";
            var displayUnits = "DisplayUnits";

            var attr = new CounterAttribute(name, type)
            {
                DisplayRateTimeScaleMs = displayRateTimeScaleMs,
                DisplayName = displayName,
                DisplayUnits = displayUnits
            };

            Assert.AreEqual(attr.Name, name);
            Assert.AreEqual(attr.Type, type);
            Assert.AreEqual(attr.DisplayRateTimeScaleMs, displayRateTimeScaleMs);
            Assert.AreEqual(attr.DisplayName, displayName);
            Assert.AreEqual(attr.DisplayUnits, displayUnits);
        }

        [TestMethod]
        public void Given_NullOrEmptyName_MustThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new CounterAttribute(null, CounterTypes.PollingCounter));
            Assert.ThrowsException<ArgumentException>(() => new CounterAttribute("", CounterTypes.PollingCounter));
        }
    }
}
