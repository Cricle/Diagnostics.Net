using Diagnostics.Traces.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class DatabaseCreatedResultBaseTest
    {
        [ExcludeFromCodeCoverage]
        class TestDatabaseCreatedResult : DatabaseCreatedResultBase
        {
            public TestDatabaseCreatedResult(string? filePath, string key) : base(filePath, key)
            {
            }

            public int DisposedCount;

            protected override void OnDisposed()
            {
                DisposedCount++;
            }
        }

        [TestMethod]
        public void GivenMustInputs()
        {
            var fp = "Test";
            var key = "k";

            var res = new TestDatabaseCreatedResult(fp, key);

            Assert.AreEqual(fp, res.FilePath);
            Assert.AreEqual(key, res.Key);
            Assert.IsNotNull(res.Root);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitWithNullKey_MustThrowArgumentNullException()
        {
            var res = new TestDatabaseCreatedResult("test", null);
        }

        [TestMethod]
        public void DefaultModeMustAll()
        {
            var res = new TestDatabaseCreatedResult("test", "key");

            Assert.AreEqual(SaveLogModes.All, res.SaveLogModes);
            Assert.AreEqual(SaveExceptionModes.All, res.SaveExceptionModes);
            Assert.AreEqual(SaveActivityModes.All, res.SaveActivityModes);
        }

        [TestMethod]
        public void Dispose_MustOnlyOne()
        {
            var res = new TestDatabaseCreatedResult("test", "key");
            res.Dispose();

            Assert.AreEqual(1, res.DisposedCount);
            res.Dispose();
            Assert.AreEqual(1, res.DisposedCount);
        }
    }
}
