using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class MapToActivityAttributeTest
    {
        [TestMethod]
        public void Given_MustEquals()
        {
            var activityClassType = typeof(object);
            var withEventSourceCall = true;
            var callEventAtEnd = true;
            var generateWithLog = true;

            var attr = new MapToActivityAttribute(activityClassType)
            {
                WithEventSourceCall = withEventSourceCall,
                CallEventAtEnd = callEventAtEnd,
                GenerateWithLog = generateWithLog
            };

            Assert.AreEqual(attr.ActivityClassType, activityClassType);
            Assert.AreEqual(attr.WithEventSourceCall, withEventSourceCall);
            Assert.AreEqual(attr.CallEventAtEnd, callEventAtEnd);
            Assert.AreEqual(attr.GenerateWithLog, generateWithLog);
        }
        [TestMethod]
        public void Given_NullType_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MapToActivityAttribute(null));
        }
    }
}
