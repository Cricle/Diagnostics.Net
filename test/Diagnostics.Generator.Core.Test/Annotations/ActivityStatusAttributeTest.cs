using Diagnostics.Generator.Core.Annotations;
using System.Diagnostics;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class ActivityStatusAttributeTest
    {
        [TestMethod]
        public void Given_MustEquals()
        {
            var code = ActivityStatusCode.Error;
            var withDescript = true;

            var attr = new ActivityStatusAttribute(code) { WithDescript = withDescript };

            Assert.AreEqual(attr.Status, code);
            Assert.AreEqual(attr.WithDescript, withDescript);
        }
    }
}
