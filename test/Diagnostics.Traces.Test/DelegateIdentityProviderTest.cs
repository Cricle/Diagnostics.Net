namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class DelegateIdentityProviderTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateIdentityProvider<int, int>(null));
        }

        [TestMethod]
        public void GivenGetter()
        {
            Func<int, GetIdentityResult<int>> getter = a => new GetIdentityResult<int>(a);

            var provider = new DelegateIdentityProvider<int, int>(getter);

            Assert.AreEqual(provider.Getter, getter);
            Assert.AreEqual(provider.GetIdentity(1).Identity, 1);
        }
    }
}
