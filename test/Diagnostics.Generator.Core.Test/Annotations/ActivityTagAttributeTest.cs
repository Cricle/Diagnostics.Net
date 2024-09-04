using Diagnostics.Generator.Core.Annotations;

namespace Diagnostics.Generator.Core.Test.Annotations
{
    [TestClass]
    public class ActivityTagAttributeTest
    {
        [TestMethod]
        public void Gieven_MustEquals()
        {
            var name = "name";
            var expression = "expression";
            var isSet = true;
            var isAdd = true;

            var attr = new ActivityTagAttribute(name, expression)
            {
                IsSet = isSet,
                IsAdd = isAdd
            };

            Assert.AreEqual(attr.Name, name);
            Assert.AreEqual(attr.Expression, expression);
            Assert.AreEqual(attr.IsSet, isSet);
            Assert.AreEqual(attr.IsAdd, isAdd);
        }

        [TestMethod]
        public void GivenNull_MustThrowArgumentNullException()
        {
            var name = "name";
            var expression = "expression";

            Assert.ThrowsException<ArgumentNullException>(() => new ActivityTagAttribute(name, null));
            Assert.ThrowsException<ArgumentNullException>(() => new ActivityTagAttribute(null, expression));
        }
    }
}
