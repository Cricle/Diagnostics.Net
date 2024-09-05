using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class BytesStoreBaseTest
    {
        [ExcludeFromCodeCoverage]
        class TestBytesStore : BytesStoreBase
        {
            public override string Name { get; } = "Test";

            public bool RaisedCount;
            public bool RaisedInsert;
            public bool RaisedInsertMany;

            public override int Count()
            {
                RaisedCount = true;
                return 0;
            }

            public override void Dispose()
            {
            }

            public override void Insert(BytesStoreValue value)
            {
                RaisedInsert = true;
            }

            public override void InsertMany(IEnumerable<BytesStoreValue> strings)
            {
                RaisedInsertMany = true;
            }
        }

        [TestMethod]
        public async Task RaiseAsync_MustCallNoAsync()
        {
            var store = new TestBytesStore();

            await store.CountAsync();
            Assert.IsTrue(store.RaisedCount);

            await store.InsertAsync(default);
            Assert.IsTrue(store.RaisedInsert);

            await store.InsertManyAsync(default);
            Assert.IsTrue(store.RaisedInsertMany);
        }
    }
}
