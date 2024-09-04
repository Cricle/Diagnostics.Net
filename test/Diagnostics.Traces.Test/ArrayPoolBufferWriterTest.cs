using System.Buffers;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class ArrayPoolBufferWriterTest
    {
        [TestMethod]
        public void InitWithDefault()
        {
            using (var writer = new ArrayPoolBufferWriter<int>())
            {
                Assert.AreEqual(writer.pool, ArrayPool<int>.Shared);
                Assert.IsTrue(writer.Capacity >= 512);
            }

            var newPool = ArrayPool<int>.Create();

            using (var writer = new ArrayPoolBufferWriter<int>(newPool))
            {
                Assert.AreEqual(writer.pool, newPool);
                Assert.IsTrue(writer.Capacity >= 512);
            }

            using (var writer = new ArrayPoolBufferWriter<int>(newPool, 1024))
            {
                Assert.AreEqual(writer.pool, newPool);
                Assert.IsTrue(writer.Capacity >= 1024);
            }
        }

        [TestMethod]
        public void InitWithNullPool_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ArrayPoolBufferWriter<int>(null, 123));
        }

        [TestMethod]
        public void Write_Advance()
        {
            using (var writer = new ArrayPoolBufferWriter<int>())
            {
                var sp = writer.GetSpan(1);
                sp[0] = 123;
                writer.Advance(1);

                Assert.AreEqual(writer.WrittenCount, 1);

                Assert.AreEqual(writer.WrittenSpan.Length, 1);
                Assert.AreEqual(writer.WrittenSpan[0], 123);

                Assert.AreEqual(writer.WrittenMemory.Length, 1);
                Assert.AreEqual(writer.WrittenMemory.Span[0], 123);
            }
        }

        [TestMethod]
        public void DiposedWirteAndAdvance_MustThrowObjectDisposedException()
        {
            var writer = new ArrayPoolBufferWriter<int>();
            writer.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => writer.Advance(1));
            Assert.ThrowsException<ObjectDisposedException>(() => writer.GetSpan(1));
            Assert.ThrowsException<ObjectDisposedException>(() => writer.GetMemory(1));
            Assert.ThrowsException<ObjectDisposedException>(() => _ = writer.WrittenMemory);
            Assert.ThrowsException<ObjectDisposedException>(() => _ = writer.WrittenSpan);
        }

        [TestMethod]
        public void AdvanceHit_MinThanZero_MustThrowArgumentOutOfRangeException()
        {
            var writer = new ArrayPoolBufferWriter<int>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Advance(-1));
        }

        [TestMethod]
        public void Write_Clear_MustReset()
        {
            var writer = new ArrayPoolBufferWriter<int>();
            var sp = writer.GetSpan(1);
            sp[0] = 123;
            writer.Advance(1);
            writer.Clear();

            Assert.AreEqual(writer.WrittenCount, 0);
            sp = writer.GetSpan(1);
            Assert.AreEqual(sp[0], 0);
        }
    }
}
