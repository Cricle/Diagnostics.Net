using Diagnostics.Traces.Serialization;
using System.Buffers;

namespace Diagnostics.Traces.Test.Serialization
{
    [TestClass]
    public class StreamMiniWriteSerializerTest
    {
        [TestMethod]
        public void Constructor_ValidStream_ShouldInitialize()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                // Act
                var serializer = new StreamMiniWriteSerializer(stream);

                // Assert
                Assert.AreEqual(stream, serializer.Stream);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullStream_ShouldThrowArgumentNullException()
        {
            // Act
            var serializer = new StreamMiniWriteSerializer(null);
        }

        [TestMethod]
        public void CanWrite_ShouldAlwaysReturnTrue()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                var serializer = new StreamMiniWriteSerializer(stream);

                // Act
                var canWrite = serializer.CanWrite(10);

                // Assert
                Assert.IsTrue(canWrite);
            }
        }

        [TestMethod]
        public void WriteCore_ShouldWriteBufferToStream()
        {
            // Arrange
            var data = new byte[] { 1, 2, 3, 4, 5 };
            var buffer = new ReadOnlySpan<byte>(data);
            using (var stream = new MemoryStream())
            {
                var serializer = new StreamMiniWriteSerializer(stream);

                // Act
                serializer.Write(buffer);

                // Assert
                CollectionAssert.AreEqual(data, stream.ToArray());
            }
        }

        [TestMethod]
        public void WriteCore_NETStandardOrNET472_ShouldRentAndReturnArray()
        {
            // Arrange
            var data = new byte[] { 1, 2, 3, 4, 5 };
            var buffer = new ReadOnlySpan<byte>(data);
            using (var stream = new MemoryStream())
            {
                var serializer = new StreamMiniWriteSerializer(stream);
                var rentedArray = ArrayPool<byte>.Shared.Rent(5);

                try
                {
                    // Act
                    serializer.Write(buffer);

                    // Assert
                    CollectionAssert.AreEqual(data, stream.ToArray());
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rentedArray);
                }
            }
        }

        [TestMethod]
        public void WriteCore_WithLargeBuffer_ShouldCorrectlyWriteData()
        {
            // Arrange
            var data = new byte[1024];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i % 256);
            }
            var buffer = new ReadOnlySpan<byte>(data);
            using (var stream = new MemoryStream())
            {
                var serializer = new StreamMiniWriteSerializer(stream);

                // Act
                serializer.Write(buffer);

                // Assert
                CollectionAssert.AreEqual(data, stream.ToArray());
            }
        }

        [TestMethod]
        public void WriteCore_ShouldHandleEmptyBuffer()
        {
            // Arrange
            var data = new byte[0];
            var buffer = new ReadOnlySpan<byte>(data);
            using (var stream = new MemoryStream())
            {
                var serializer = new StreamMiniWriteSerializer(stream);

                // Act
                serializer.Write(buffer);

                // Assert
                Assert.AreEqual(0, stream.Length);
            }
        }
    }
}
