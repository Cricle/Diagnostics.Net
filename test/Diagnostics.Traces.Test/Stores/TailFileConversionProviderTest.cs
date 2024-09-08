using Diagnostics.Traces.Stores;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class TailFileConversionProviderTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TailFileConversionProvider(null));
        }

        [TestMethod]
        public void GivenTail_CallMustAddTail()
        {
            var tail = "_tail";
            var provider = new TailFileConversionProvider(tail);
            var fp = AppContext.BaseDirectory;
            var res = provider.ConvertPath(fp);

            Assert.AreEqual(fp + tail, res);
        }
    }
}
