using Diagnostics.Traces.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test.Serialization
{
    [TestClass]
    public class BufferMiniWriteSerializerTest
    {
        [ExcludeFromCodeCoverage]
        class TestBufferMiniWriteSerializer : BufferMiniWriteSerializer
        {
            public int DisposedCount;

            public Func<int, bool>? CanWriteFun;

            public List<byte> Writted { get; } = new List<byte>();

            public override bool CanWrite(int length)
            {
                return CanWriteFun?.Invoke(length) ?? true;
            }

            protected override void WriteCore(ReadOnlySpan<byte> buffer)
            {
                Writted.AddRange(buffer);
            }

            protected override void OnDisposed()
            {
                DisposedCount++;
            }
        }
        [TestMethod]
        public void ScopeWrite()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            Assert.IsTrue(ser.TryEntryScope(1024));
            Assert.IsTrue(ser.IsEntryScoped);

            ReadOnlySpan<byte> buffer = new byte[] { 1, 2, 3, 4 };
            ser.Write(buffer);
            ser.FlushScope();

            Assert.AreEqual(ser.Writted.Count, 4);
            Assert.IsTrue(ser.Writted.SequenceEqual(Enumerable.Range(1, 4).Select(x => (byte)x)));
        }

        [TestMethod]
        public void ScopeEntry_WhenDupEntry_ReturnFail()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            Assert.IsTrue(ser.TryEntryScope(1));
            Assert.IsFalse(ser.TryEntryScope(1));
        }

        [TestMethod]
        public void ScopeEntry_Flush_MustExitScop()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            Assert.IsTrue(ser.TryEntryScope(1));
            Assert.IsTrue(ser.FlushScope());
            Assert.AreEqual(ser.Writted.Count, 0);
            Assert.IsFalse(ser.IsEntryScoped);
        }

        [TestMethod]
        public void Delete_WhenNotInScope_Return_False()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            Assert.IsFalse(ser.DeleteScope());
            Assert.AreEqual(ser.Writted.Count, 0);
        }

        [TestMethod]
        public void Flush_WhenNotInScope_Return_False()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            Assert.IsFalse(ser.DeleteScope());
            Assert.AreEqual(ser.Writted.Count, 0);
        }

        [TestMethod]
        public void ScopeBuffer_Check()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            Assert.IsTrue(ser.TryEntryScope(1));
            ser.Write(new byte[] {1});
            var buffer = ser.GetScopedBuffer();
            Assert.AreEqual(buffer.Length, 1);
            Assert.AreEqual(buffer[0], 1);
        }

        [TestMethod]
        public void NotInScope_ScopeBuffer_Empty()
        {
            using var ser = new TestBufferMiniWriteSerializer();
            var buffer = ser.GetScopedBuffer();
            Assert.AreEqual(buffer.Length, 0);
            Assert.IsTrue(buffer == Span<byte>.Empty);
        }

        [TestMethod]
        public void DisposedMustOnly()
        {
            var ser = new TestBufferMiniWriteSerializer();
            ser.Dispose();
            Assert.AreEqual(ser.DisposedCount, 1);
            ser.Dispose();
            Assert.AreEqual(ser.DisposedCount, 1);
        }

        [TestMethod]
        public void CanWrite_Not_ThrowInvalidOperationException()
        {
            var ser = new TestBufferMiniWriteSerializer
            {
                CanWriteFun = _ => false
            };
            Assert.ThrowsException<InvalidOperationException>(() => ser.Write(new byte[] { 1 }));
        }
    }
}
