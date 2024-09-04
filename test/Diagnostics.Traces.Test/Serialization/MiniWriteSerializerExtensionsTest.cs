using Diagnostics.Traces.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Diagnostics.Traces.Test.Serialization
{
    [TestClass]
    public unsafe class MiniWriteSerializerExtensionsTest
    {
        [TestMethod]
        public void WriteStruct()
        {
            using var s = new DefaultWritableBuffer();
            s.Write(123);

            Assert.AreEqual(s.BufferWriter.WrittenCount, sizeof(int));
            Assert.AreEqual(Unsafe.Read<int>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(s.BufferWriter.WrittenSpan))), 123);
        }

        [TestMethod]
        public void WriteString_Null()
        {
            using var s = new DefaultWritableBuffer();
            s.Write((string?)null);

            Assert.AreEqual(s.BufferWriter.WrittenCount, sizeof(int));
            Assert.AreEqual(Unsafe.Read<int>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(s.BufferWriter.WrittenSpan))), -1);
        }

        [TestMethod]
        public void WriteString_Empty()
        {
            using var s = new DefaultWritableBuffer();
            s.Write(string.Empty);

            Assert.AreEqual(s.BufferWriter.WrittenCount, sizeof(int));
            Assert.AreEqual(Unsafe.Read<int>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(s.BufferWriter.WrittenSpan))), 0);
        }

        [TestMethod]
        public void WriteString()
        {
            WriteStringCore("str");
        }

        [TestMethod]
        public void WriteString_Long()
        {
            WriteStringCore(RandomStringHelper.CreateRandomString(1024 * 2));
        }

        private void WriteStringCore(string str)
        {
            var strBuffer = Encoding.UTF8.GetBytes(str);

            using var s = new DefaultWritableBuffer();
            s.Write(str);

            Assert.AreEqual(s.BufferWriter.WrittenCount, sizeof(int) + strBuffer.Length);
            Assert.AreEqual(Unsafe.Read<int>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(s.BufferWriter.WrittenSpan))), strBuffer.Length);

            var retStr = Encoding.UTF8.GetString(s.BufferWriter.WrittenSpan.Slice(sizeof(int)).ToArray());

            Assert.AreEqual(retStr, str);
        }

        [TestMethod]
        public void WriteNull_MustEqualsStringNull()
        {
            using var s = new DefaultWritableBuffer();
            s.WriteNull();
            Assert.AreEqual(s.BufferWriter.WrittenCount, sizeof(int));
            Assert.AreEqual(Unsafe.Read<int>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(s.BufferWriter.WrittenSpan))), -1);
        }
    }
}
