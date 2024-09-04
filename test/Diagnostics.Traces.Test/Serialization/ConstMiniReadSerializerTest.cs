using Diagnostics.Traces.Serialization;

namespace Diagnostics.Traces.Test.Serialization
{
    [TestClass]
    public unsafe class ConstMiniReadSerializerTest
    {
        [TestMethod]
        public void GivenNullptr_MustThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ConstMiniReadSerializer(null, 1));
        }

        [TestMethod]
        public void GivenMinOrEqualThanZeroLength_MustThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var buffer = new byte[1];
                fixed (byte* bufferptr = buffer)
                {
                    new ConstMiniReadSerializer(bufferptr, 0);
                }
            });
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var buffer = new byte[1];
                fixed (byte* bufferptr = buffer)
                {
                    new ConstMiniReadSerializer(bufferptr, -1);
                }
            });
        }

        [TestMethod]
        public void GivenBuffer_ReadMustOutputGiven()
        {
            var buffer = new byte[sizeof(int)];
            BitConverter.TryWriteBytes(buffer, 102444);
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);
                Assert.IsTrue(ser.CanSeek);
                for (int i = 0; i < 5; i++)
                {
                    Assert.IsTrue(ser.CanRead(i));
                }
                var newBuffer = new byte[sizeof(int)];
                ser.Read(newBuffer);
                Assert.IsTrue(newBuffer.SequenceEqual(buffer));
            }
        }

        [TestMethod]
        public void CanRead_GivenMinThanZero_MustThrowArgumentException()
        {
            var buffer = new byte[sizeof(int)];
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);
                Assert.ThrowsException<ArgumentException>(() => ser.CanRead(-1));
            }
        }

        [TestMethod]
        public void ReadOutOfRange_MustThrowArgumentOutOfRangeException()
        {
            var buffer = new byte[sizeof(int)];
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);
                var newBuffer = new byte[buffer.Length + 1];
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => ser.Read(newBuffer));
            }
        }

        [TestMethod]
        public void Read_Offset()
        {
            var buffer = new byte[] { 1, 2, 3, 4 };
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);
                Assert.AreEqual(ser.BuffetLength, buffer.Length);

                var newBuffer = new byte[buffer.Length + 1];
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => ser.Read(newBuffer));

                newBuffer = new byte[1] { 0 };
                ser.Read(newBuffer);
                Assert.AreEqual(newBuffer[0], 1);
                for (int i = 0; i < 4; i++)
                {
                    Assert.IsTrue(ser.CanRead(i));
                }
                Assert.IsFalse(ser.CanRead(4));

                ser.Read(newBuffer);
                Assert.AreEqual(newBuffer[0], 2);
                for (int i = 0; i < 3; i++)
                {
                    Assert.IsTrue(ser.CanRead(i));
                }
                Assert.IsFalse(ser.CanRead(3));


                ser.Read(newBuffer);
                Assert.AreEqual(newBuffer[0], 3);
                for (int i = 0; i < 2; i++)
                {
                    Assert.IsTrue(ser.CanRead(i));
                }
                Assert.IsFalse(ser.CanRead(2));

                ser.Read(newBuffer);
                Assert.AreEqual(newBuffer[0], 4);
                for (int i = 0; i < 1; i++)
                {
                    Assert.IsTrue(ser.CanRead(i));
                }
                Assert.IsFalse(ser.CanRead(1));
            }
        }

        [TestMethod]
        public void Skip_MustAddOffset()
        {
            var buffer = new byte[] { 1, 2, 3, 4 };
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);
                ser.Skip(1);
                Assert.AreEqual(ser.Offset, 1);

                var newBuffer = new byte[1];
                ser.Read(newBuffer);
                Assert.AreEqual(newBuffer[0], 2);
            }
        }

        [TestMethod]
        public void Skip_OutOfRange_MustThrowArgumentOutOfRangeException()
        {
            var buffer = new byte[] { 1, 2, 3, 4 };
            fixed (byte* ptr = buffer)
            {
                var ser = new ConstMiniReadSerializer(ptr, buffer.Length);
                Assert.ThrowsException<ArgumentOutOfRangeException>(()=> ser.Skip(5));
            }
        }
    }
}
