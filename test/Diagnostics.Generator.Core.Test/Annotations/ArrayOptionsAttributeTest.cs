using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class ArrayOptionsAttributeTest
    {
        [TestMethod]
        public void Given_MustEquals()
        {
            var options = ArrayOptions.Join;

            var attr = new ArrayOptionsAttribute(options);

            Assert.AreEqual(attr.Options, options);
        }
    }
}
