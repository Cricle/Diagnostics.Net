using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class BytesStoreManagerBaseTest
    {
        [ExcludeFromCodeCoverage]
        class NullBytesStore : IBytesStore
        {
            public string Name { get; set; }

            public int Count()
            {
                return 0;
            }

            public Task<int> CountAsync()
            {
                return Task.FromResult(0);
            }

            public void Dispose()
            {
            }

            public void Insert(BytesStoreValue value)
            {
            }

            public Task InsertAsync(BytesStoreValue value)
            {
                return Task.CompletedTask;
            }

            public void InsertMany(IEnumerable<BytesStoreValue> strings)
            {
            }

            public Task InsertManyAsync(IEnumerable<BytesStoreValue> strings)
            {
                return Task.CompletedTask;
            }
        }
        [ExcludeFromCodeCoverage]
        class TestBytesStoreManager : BytesStoreManagerBase
        {
            protected override IBytesStore CreateStringStore(string name)
            {
                return new NullBytesStore { Name = name };
            }
        }
        [TestMethod]
        public void GetOrAdd()
        {
            using var mgr = new TestBytesStoreManager();
            var s1 = mgr.GetOrAdd("test");
            var s2 = mgr.GetOrAdd("test");
            var s3 = mgr.GetOrAdd("test1");
            Assert.AreEqual(s2, s1);
            Assert.AreNotEqual(s3, s1);

            Assert.AreEqual(mgr.Count, 2);
            Assert.AreEqual(2, mgr.Count());
        }

        [TestMethod]
        public void Exists()
        {
            using var mgr = new TestBytesStoreManager();
            Assert.IsFalse(mgr.Exists("test"));
            var s1 = mgr.GetOrAdd("test");
            Assert.IsTrue(mgr.Exists("test"));
        }

        [TestMethod]
        public void Remove()
        {
            using var mgr = new TestBytesStoreManager();
            Assert.IsFalse(mgr.Remove("test"));
            var s1 = mgr.GetOrAdd("test");
            Assert.IsTrue(mgr.Remove("test"));

            Assert.AreEqual(mgr.Count, 0);
        }
    }
}
