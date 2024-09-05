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
            Assert.ThrowsException<ObjectDisposedException>(() => _ = writer.Capacity);
            Assert.ThrowsException<ObjectDisposedException>(() => _ = writer.FreeCapacity);
            Assert.ThrowsException<ObjectDisposedException>(() => writer.Clear());
        }

        [TestMethod]
        public void AdvanceHit_MinThanZero_MustThrowArgumentOutOfRangeException()
        {
            using var writer = new ArrayPoolBufferWriter<int>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Advance(-1));
        }

        [TestMethod]
        public void Write_Clear_MustReset()
        {
            using var writer = new ArrayPoolBufferWriter<int>();
            var sp = writer.GetSpan(1);
            sp[0] = 123;
            writer.Advance(1);
            writer.Clear();

            Assert.AreEqual(writer.WrittenCount, 0);
            sp = writer.GetSpan(1);
            Assert.AreEqual(sp[0], 0);
        }

        [TestMethod]
        public void AddWithResize()
        {
            using var writer = new ArrayPoolBufferWriter<int>();
            var size = writer.FreeCapacity;
            writer.Advance(size);
            var sp = writer.GetSpan(1);
            sp[0] = 1;
            writer.Advance(1);

            Assert.AreEqual(writer.WrittenCount, size + 1);
            Assert.AreNotEqual(writer.FreeCapacity, 0);
            Assert.AreEqual(writer.FreeCapacity, writer.Capacity - writer.WrittenCount);
        }

        [TestMethod]
        public void AppendChars_ToStringMustReturnString()
        {
            using var writer = new ArrayPoolBufferWriter<char>();
            var sp = writer.GetSpan(5);
            "12345".AsSpan().CopyTo(sp);
            writer.Advance(5);

            Assert.AreEqual(writer.ToString(), "12345");
        }

        [TestMethod]
        public void AppendNoChars_ToStringMustReturnString()
        {
            using var writer = new ArrayPoolBufferWriter<int>();
            Assert.AreEqual(writer.ToString(), $"ArrayPoolBufferWriter<{typeof(int)}>[0]");

            writer.Advance(1);
            Assert.AreEqual(writer.ToString(), $"ArrayPoolBufferWriter<{typeof(int)}>[1]");
        }
    }
}
