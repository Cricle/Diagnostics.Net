using Diagnostics.Traces.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test.Serialization
{
    [TestClass]
    public class StreamMiniReadSerializerTest
    {
        [ExcludeFromCodeCoverage]
        class NonSeekableStream : MemoryStream
        {
            public override bool CanSeek => false;
        }

        [TestMethod]
        public void Constructor_ValidStream_ShouldInitialize()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                // Act
                var serializer = new StreamMiniReadSerializer(stream);

                // Assert
                Assert.AreEqual(stream, serializer.Stream);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullStream_ShouldThrowArgumentNullException()
        {
            // Act
            var serializer = new StreamMiniReadSerializer(null);
        }

        [TestMethod]
        public void CanSeek_ShouldReturnTrueIfStreamCanSeek()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                var serializer = new StreamMiniReadSerializer(stream);

                // Act
                var canSeek = serializer.CanSeek;

                // Assert
                Assert.IsTrue(canSeek);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanRead_IfStreamCannotSeek_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var nonSeekableStream = new NonSeekableStream();
            var serializer = new StreamMiniReadSerializer(nonSeekableStream);

            // Act
            serializer.CanRead(10);
        }

        [TestMethod]
        public void CanRead_IfLengthIsLessThanPositionPlusLength_ShouldReturnTrue()
        {
            // Arrange
            using (var stream = new MemoryStream(new byte[100]))
            {
                var serializer = new StreamMiniReadSerializer(stream);

                // Act
                var canRead = serializer.CanRead(50);

                // Assert
                Assert.IsTrue(canRead);
            }
        }

        [TestMethod]
        public void CanRead_IfLengthExceedsStreamLength_ShouldReturnFalse()
        {
            // Arrange
            using (var stream = new MemoryStream(new byte[100]))
            {
                var serializer = new StreamMiniReadSerializer(stream);

                // Act
                var canRead = serializer.CanRead(150);

                // Assert
                Assert.IsFalse(canRead);
            }
        }

        [TestMethod]
        public void Read_ShouldReadFromStreamIntoBuffer()
        {
            // Arrange
            var data = new byte[] { 1, 2, 3, 4, 5 };
            using (var stream = new MemoryStream(data))
            {
                var serializer = new StreamMiniReadSerializer(stream);
                var buffer = new byte[5];

                // Act
                serializer.Read(buffer);

                // Assert
                CollectionAssert.AreEqual(data, buffer);
            }
        }

        [TestMethod]
        public void Dispose_ShouldDisposeStream()
        {
            // Arrange
            var stream = new MemoryStream();
            var serializer = new StreamMiniReadSerializer(stream);

            // Act
            serializer.Dispose();

            // Assert
            Assert.ThrowsException<ObjectDisposedException>(() => stream.ReadByte());
        }
    }
}
