namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class GetIdentityResultTest
    {
        [TestMethod]
        public void Fail_Const()
        {
            Assert.AreEqual(GetIdentityResult<int>.Fail.Identity, default);
            Assert.IsFalse(GetIdentityResult<int>.Fail.Succeed);
        }

        [TestMethod]
        public void SucceedEmpty_Const()
        {
            Assert.AreEqual(GetIdentityResult<int>.SucceedEmptyIdentity.Identity, default);
            Assert.IsTrue(GetIdentityResult<int>.SucceedEmptyIdentity.Succeed);
        }

        [TestMethod]
        public void Initial()
        {
            var res = new GetIdentityResult<int>(123, true);
            Assert.AreEqual(res.Identity, 123);
            Assert.IsTrue(res.Succeed); 

            res = new GetIdentityResult<int>(123);
            Assert.AreEqual(res.Identity, 123);
            Assert.IsTrue(res.Succeed);
        }
    }
}
