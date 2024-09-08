using Diagnostics.Traces.Stores;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class StartWithLastWriteFileDeleteRulesTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StartWithLastWriteFileDeleteRules(null, 100));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void InitKeepFileCountMinThanOne_MustThrowArgumentOutOfRangeException(int count)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new StartWithLastWriteFileDeleteRules("a", count));
        }

    }
}
