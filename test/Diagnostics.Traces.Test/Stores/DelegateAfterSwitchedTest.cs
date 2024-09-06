using Diagnostics.Traces.Stores;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class DelegateAfterSwitchedTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateAfterSwitched<object>(null));
        }

        [TestMethod]
        public void CallMustInvokeAction()
        {
            var called = false;
            var val = new object();
            object? calledObject = null;
            var switcher = new DelegateAfterSwitched<object>(o =>
            {
                calledObject = o;
                called = true;
            });

            switcher.AfterSwitched(val);

            Assert.IsTrue(called);
            Assert.AreEqual(val, calledObject);
        }
    }
}
