using Diagnostics.Traces.Status;

namespace Diagnostics.Traces.Test.Status
{
    [TestClass]
    public class ValueListBuilderTest
    {
        [TestMethod]
        public void Add_WithNoGrow()
        {
            using (var list = new ValueListBuilder<int>())
            {
                list.Append(1);
                list.Append(2);

                Assert.AreEqual(list.Length, 2);
            }
        }

        [TestMethod]
        public void Add_WithGrow()
        {
            using (var list = new ValueListBuilder<int>())
            {
                list.Append(0);
                var count = list._span!.Length;
                for (int i = 0; i < list._span.Length-1; i++)
                {
                    list.Append(i + 1);
                }
                list.Append(count);
                count++;

                Assert.AreEqual(list.Length, count);
                Assert.IsTrue(list.AsSpan().SequenceEqual(Enumerable.Range(0, count).ToArray()));
            }
        }

        [TestMethod]
        public void ByRefIndex()
        {
            using (var list = new ValueListBuilder<int>())
            {
                list.Append(1024);
                ref int value = ref list[0];
                Assert.AreEqual(value, 1024);

                value = 456;
                Assert.AreEqual(value, 456);
                Assert.AreEqual(list.Length, 1);
            }
        }

        [TestMethod]
        public void AppendBySpan()
        {
            using (var list = new ValueListBuilder<int>())
            {
                ReadOnlySpan<int> buffer = new int[] {0, 1, 2, 3, 4 };

                list.Append(buffer);

                Assert.AreEqual(list.Length, 5);
                Assert.IsTrue(list.AsSpan().SequenceEqual(Enumerable.Range(0, 5).ToArray()));
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TryCopyFail(int index)
        {
            using (var list = new ValueListBuilder<int>())
            {
                ReadOnlySpan<int> buffer = new int[] { 0, 1, 2, 3, 4 };
                list.Append(buffer);
                Span<int> copyBuffer = new int[index];

                Assert.IsFalse(list.TryCopyTo(copyBuffer, out var itemsWritten));
                Assert.AreEqual(itemsWritten, 0);
            }
        }
        [TestMethod]
        [DataRow(5)]
        [DataRow(6)]
        public void TryCopySucceed(int index)
        {
            using (var list = new ValueListBuilder<int>())
            {
                ReadOnlySpan<int> buffer = new int[] { 0, 1, 2, 3, 4 };
                list.Append(buffer);
                Span<int> copyBuffer = new int[index];

                Assert.IsTrue(list.TryCopyTo(copyBuffer, out var itemsWritten));
                Assert.AreEqual(itemsWritten, list.Length);
                Assert.IsTrue(list.AsSpan().SequenceEqual(Enumerable.Range(0, list.Length).ToArray()));
            }
        }
    }
}
