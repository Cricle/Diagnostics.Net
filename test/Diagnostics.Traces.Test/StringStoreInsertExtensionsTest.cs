using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class StringStoreInsertExtensionsTest
    {
        [ExcludeFromCodeCoverage]
        class TestBytesStore : BytesStoreBase
        {
            public override string Name { get; }

            public List<BytesStoreValue> Values { get; } = new List<BytesStoreValue>();

            public override int Count()
            {
                return 0;
            }

            public override void Dispose()
            {
            }

            public override void Insert(BytesStoreValue value)
            {
                Values.Add(value);
            }

            public override void InsertMany(IEnumerable<BytesStoreValue> strings)
            {
                Values.AddRange(strings);
            }
        }

        private string DecGzip(in BytesStoreValue value)
        {
            using (var mem = new MemoryStream(value.Value, value.Offset, value.Length))
            using (var gzip = new GZipStream(mem, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzip))
            {
                return reader.ReadToEnd();
            }
        }

        [TestMethod]
        public void InsertString()
        {
            var store = new TestBytesStore();
            StringStoreInsertExtensions.Insert(store, "test");

            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(Encoding.UTF8.GetString(store.Values[0].Value), "test");
        }

        [TestMethod]
        public async Task InsertStringAsync()
        {
            var store = new TestBytesStore();
            await StringStoreInsertExtensions.InsertAsync(store, "test");

            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(Encoding.UTF8.GetString(store.Values[0].Value), "test");
        }

        [TestMethod]
        public void InsertMany()
        {
            var store = new TestBytesStore();

            var datas = Enumerable.Range(0, 10).Select(x => "test" + x).ToArray();

            StringStoreInsertExtensions.InsertMany(store, datas);

            Assert.AreEqual(store.Values.Count, datas.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                Assert.AreEqual(Encoding.UTF8.GetString(store.Values[i].Value), "test" + i);
            }
        }

        [TestMethod]
        public async Task InsertManyAsync()
        {
            var store = new TestBytesStore();

            var datas = Enumerable.Range(0, 10).Select(x => "test" + x).ToArray();

            await StringStoreInsertExtensions.InsertManyAsync(store, datas);

            Assert.AreEqual(store.Values.Count, datas.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                Assert.AreEqual(Encoding.UTF8.GetString(store.Values[i].Value), "test" + i);
            }
        }

        [TestMethod]
        public void InsertGzip()
        {
            var store = new TestBytesStore();

            var data = "test gzip";

            StringStoreInsertExtensions.InsertGzip(store, data);
            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(DecGzip(store.Values[0]), data);
            store.Values.Clear();


            var newBuffer = Encoding.UTF8.GetBytes(data);
            StringStoreInsertExtensions.InsertGzip(store, newBuffer);
            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(DecGzip(store.Values[0]), data);
            store.Values.Clear();


            var newOutterBuffer = new byte[newBuffer.Length + 1];
            newBuffer.CopyTo(newOutterBuffer.AsSpan(1));
            StringStoreInsertExtensions.InsertGzip(store, newOutterBuffer, 1, newBuffer.Length);
            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(DecGzip(store.Values[0]), data);
            store.Values.Clear();
        }

        [TestMethod]
        public async Task InsertGzipAsync()
        {
            var store = new TestBytesStore();

            var data = "test gzip";

            await StringStoreInsertExtensions.InsertGzipAsync(store, data);
            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(DecGzip(store.Values[0]), data);
            store.Values.Clear();


            var newBuffer = Encoding.UTF8.GetBytes(data);
            await StringStoreInsertExtensions.InsertGzipAsync(store, newBuffer);
            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(DecGzip(store.Values[0]), data);
            store.Values.Clear();


            var newOutterBuffer = new byte[newBuffer.Length + 1];
            newBuffer.CopyTo(newOutterBuffer.AsSpan(1));
            await StringStoreInsertExtensions.InsertGzipAsync(store, newOutterBuffer, 1, newBuffer.Length);
            Assert.AreEqual(store.Values.Count, 1);
            Assert.AreEqual(DecGzip(store.Values[0]), data);
            store.Values.Clear();
        }
    }
}
