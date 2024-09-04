using System.Buffers;
using System.Collections;

namespace Diagnostics.Generator.Core.Test
{
    [TestClass]
    public class BatchDataTest
    {
        [TestMethod]
        public void GivenData_Disposed()
        {
            const int size = 1024;

            var buffer = ArrayPool<int>.Shared.Rent(size);
            var data = new BatchData<int>(buffer, size);

            Assert.AreEqual(data.datas, buffer);
            Assert.AreEqual(data.DangerousGetDatas(), buffer);
            Assert.AreEqual(data.Count, size);
            Assert.AreEqual(data.Datas.Length, size);

            data.Dispose();
        }
        [TestMethod]
        public void GivenData_Enumerable_MustEquals()
        {
            const int size = 1024;

            var buffer = ArrayPool<int>.Shared.Rent(size);
            var data = new BatchData<int>(buffer, size);

            for (int i = 0; i < size; i++)
            {
                buffer[i] = i;
            }

            Assert.AreEqual(data.Count(), size);
            Assert.IsTrue(data.SequenceEqual(buffer));
            Assert.IsTrue(data.Datas.SequenceEqual(buffer.AsSpan(0, size)));

            var enu = ((IEnumerable)data).GetEnumerator();
            var idx = 0;
            while (enu.MoveNext())
            {
                Assert.AreEqual(enu.Current, idx);
                idx++;
            }
        }
        [TestMethod]
        public void EmptyTest()
        {
            BatchData<int> empty = default;

            Assert.AreEqual(empty.Count, 0);
            Assert.IsTrue(empty.Datas == Span<int>.Empty);
            Assert.IsFalse(empty.Any());

            empty.Dispose();
        }
    }
}
