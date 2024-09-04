using Diagnostics.Traces.Serialization;
using System.Text;

namespace Diagnostics.Traces.Test.Serialization
{

    [TestClass]
    public unsafe class MiniReadSerializerExtensionsTest
    {
        [TestMethod]
        public void ReadStruct()
        {
            const int value = 102444;

            var buffer = new byte[sizeof(int)];
            BitConverter.TryWriteBytes(buffer, value);
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);

                var val = MiniReadSerializerExtensions.Read<int>(ser);

                Assert.AreEqual(val, value);
            }
        }

        [TestMethod]
        public void ReadString_Null()
        {
            var buffer = new byte[sizeof(int)];
            BitConverter.TryWriteBytes(buffer, -1);
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);

                var val = MiniReadSerializerExtensions.ReadString(ser);

                Assert.IsNull(val);
            }
        }

        [TestMethod]
        public void ReadString_Empty()
        {
            var buffer = new byte[sizeof(int)];
            BitConverter.TryWriteBytes(buffer, 0);
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);

                var val = MiniReadSerializerExtensions.ReadString(ser);

                Assert.AreEqual(val, string.Empty);
            }
        }

        [TestMethod]
        public void ReadString()
        {
            ReadStringCore("hello");
        }
        private void ReadStringCore(string str)
        {
            var strBuffer = Encoding.UTF8.GetBytes(str);
            var buffer = new byte[sizeof(int) + strBuffer.Length];
            BitConverter.TryWriteBytes(buffer, strBuffer.Length);
            strBuffer.CopyTo(buffer.AsSpan(sizeof(int)));
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);

                var val = MiniReadSerializerExtensions.ReadString(ser);

                Assert.AreEqual(val, str);
            }
        }
        [TestMethod]
        public void ReadLongString()
        {
            ReadStringCore(RandomStringHelper.CreateRandomString(1024 * 2));
        }
    }
}
