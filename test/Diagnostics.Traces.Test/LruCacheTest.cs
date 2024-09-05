using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class LruCacheTest
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void InitMinThanMinimun_MustThrowArgumentException(int count)
        {
            Assert.ThrowsException<ArgumentException>(() => new LruCache<object, object>(count));
        }

        [TestMethod]
        public void CapacitySameAsInput()
        {
            using var lru = new LruCache<int, int>(100);

            Assert.AreEqual(lru.Capacity, 100);
            Assert.AreEqual(lru.Count, 0);
            Assert.IsFalse(lru.IsReadOnly);
        }

        [TestMethod]
        public void Add_NotExists_MustBeAdd()
        {
            using var lru = new LruCache<int, int>(100);

            lru.Add(1, 1);
            Assert.AreEqual(lru.Count, 1);
            Assert.AreEqual(lru.data.Keys.Single(), 1);
            Assert.AreEqual(lru.data.Values.Single().Value, 1);
        }

        [TestMethod]
        public void AddOrUpdate_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.AddOrUpdate(null, 1));
            Assert.ThrowsException<ArgumentNullException>(() => lru.AddOrUpdate(1, (Func<object, object>)null!));
            Assert.ThrowsException<ArgumentNullException>(() => lru.AddOrUpdate(null, _ => _));
        }

        [TestMethod]
        public void GetOrAdd_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.GetOrAdd(null, _ => _));
            Assert.ThrowsException<ArgumentNullException>(() => lru.GetOrAdd(1, null));
        }

        [TestMethod]
        public void TryPeek_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.TryPeek(null, out _));
        }

        [TestMethod]
        public void TryGetValue_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.TryGetValue(null, out _));
        }

        [TestMethod]
        public void Remove_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.TryRemove(null, out _));
        }

        [TestMethod]
        public void Index_NotFound_MustThrowKeyNotFoundException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<KeyNotFoundException>(() => lru[1]);
        }

        [TestMethod]
        public void ContainsKey_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.ContainsKey(null));
        }

        [TestMethod]
        public void Add_GivenNull_MustThrowArgumentNullException()
        {
            using var lru = new LruCache<object, object>(100);

            Assert.ThrowsException<ArgumentNullException>(() => lru.Add(null, 1));
            Assert.ThrowsException<ArgumentNullException>(() => lru.Add(new KeyValuePair<object, object>(null, 1)));
        }

        [TestMethod]
        public void Add_WithSwitch()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(1, 1);
            lru.Add(2, 2);
            lru.Add(3, 3);

            Assert.AreEqual(lru.Count, 2);
            Assert.IsFalse(lru.ContainsKey(1));
            Assert.IsTrue(lru.ContainsKey(2));
            Assert.IsTrue(lru.ContainsKey(3));
            Assert.IsTrue(lru.TryGetValue(2, out var val));
            Assert.AreEqual(val, 2);
            Assert.IsTrue(lru.TryGetValue(3, out val));
            Assert.AreEqual(val, 3);
            Assert.IsFalse(lru.TryGetValue(1, out _));
        }

        [TestMethod]
        public void TryPeek()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(1, 1);
            lru.Add(2, 2);
            lru.TryPeek(1, out _);
            lru.Add(3, 3);

            Assert.AreEqual(lru.Count, 2);
            Assert.IsFalse(lru.ContainsKey(1));
            Assert.IsTrue(lru.ContainsKey(2));
            Assert.IsTrue(lru.ContainsKey(3));
            Assert.IsTrue(lru.TryGetValue(2, out var val));
            Assert.AreEqual(val, 2);
            Assert.IsTrue(lru.TryGetValue(3, out val));
            Assert.AreEqual(val, 3);
            Assert.IsFalse(lru.TryGetValue(1, out _));
        }

        [TestMethod]
        public void TryPeek_NotExists_MustReturnFail()
        {
            using var lru = new LruCache<int, int>(2);

            Assert.IsFalse(lru.TryPeek(1, out _));

        }

        [TestMethod]
        public void IndexVisit()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(1, 1);
            lru.Add(2, 2);

            Assert.AreEqual(lru[1], 1);
            Assert.AreEqual(lru[2], 2);
        }

        [TestMethod]
        public void IndexSet_Visit()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(1, 1);
            lru[1] = 2;

            Assert.AreEqual(lru[1], 2);

            lru[2] = 3;
            Assert.AreEqual(lru[2], 3);
        }

        [TestMethod]
        public void Add_ButKeyExists_WillReplace()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(1, 1);
            lru.Add(1, 2);

            Assert.AreEqual(2, lru[1]);
        }

        [TestMethod]
        public void AddOrUpdate_ButKeyExists_WillReplace()
        {
            using var lru = new LruCache<int, int>(2);
            lru.AddOrUpdate(1, k => 1);
            Assert.AreEqual(lru[1], 1);

            lru.AddOrUpdate(1, k => 2);
            Assert.AreEqual(lru[1], 2);
        }

        [TestMethod]
        public void Clear_AllMustReset()
        {
            using var lru = new LruCache<int, int>(2);
            lru.AddOrUpdate(1, k => 1);
            lru.Clear();

            Assert.AreEqual(lru.Count, 0);
        }

        [TestMethod]
        public void GetOrAdd()
        {
            using var lru = new LruCache<int, int>(2);
            var res = lru.GetOrAdd(1, k => 1);
            Assert.AreEqual(res, 1);

            res = lru.GetOrAdd(1, k => 1);
            Assert.AreEqual(res, 1);
            Assert.AreEqual(lru.Count, 1);
        }

        [TestMethod]
        public void TryRemove_NothingWillFail()
        {
            using var lru = new LruCache<int, int>(2);
            Assert.IsFalse(lru.TryRemove(1, out _));
        }

        [TestMethod]
        public void TryRemove_ContainsWillTrue()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(1, 1);
            Assert.IsTrue(lru.TryRemove(1, out var val));
            Assert.AreEqual(val, 1);
        }

        [TestMethod]
        public void AddWithKeyValuePair()
        {
            using var lru = new LruCache<int, int>(2);
            lru.Add(new KeyValuePair<int, int>(1, 1));

            Assert.AreEqual(lru[1], 1);
            Assert.AreEqual(lru.Count, 1);
        }

        [TestMethod]
        public void Remove_Center()
        {
            using var lru = new LruCache<int, int>(5);
            for (int i = 0; i < 5; i++)
            {
                lru.Add(i, i);
            }

            Assert.AreEqual(lru.Count, 5);
            Assert.IsTrue(lru.TryRemove(3, out var val));
            Assert.AreEqual(val, 3);

            for (int i = 0; i < 5; i++)
            {
                if (i == 3)
                {
                    Assert.IsFalse(lru.ContainsKey(i));
                }
                else
                {
                    Assert.IsTrue(lru.ContainsKey(i));
                    Assert.AreEqual(lru[i], i);
                }
            }
        }
    }
}
