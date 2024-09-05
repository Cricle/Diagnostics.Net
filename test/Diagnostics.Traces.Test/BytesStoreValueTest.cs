using System.Text;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class BytesStoreValueTest
    {
        private string AsString(in BytesStoreValue value)
        {
            return Encoding.UTF8.GetString(value.Value, value.Offset, value.Length);
        }

        [TestMethod]
        public void InitWithString()
        {
            var value = new BytesStoreValue("test");
            Assert.AreEqual(AsString(value), "test");
        }

        [TestMethod]
        public void InitWithImplicit()
        {
            BytesStoreValue value = "test";
            Assert.AreEqual(AsString(value), "test");

            value = Encoding.UTF8.GetBytes("test");
            Assert.AreEqual(AsString(value), "test");
        }

        [TestMethod]
        public void InitWithString_WithTime()
        {
            var time = DateTime.Now;

            var value = new BytesStoreValue(time, "test");

            Assert.AreEqual(AsString(value), "test");
            Assert.AreEqual(value.Time, time);
        }

        [TestMethod]
        public void InitWithBytes()
        {
            var time = DateTime.Now;

            var buffer = new byte[5];

            var value = new BytesStoreValue(time, buffer);

            Assert.AreEqual(value.Value, buffer);
            Assert.AreEqual(value.Offset, 0);
            Assert.AreEqual(value.Length, buffer.Length);
            Assert.AreEqual(value.Time, time);
        }

        [TestMethod]
        public void InitWithBytesOffset()
        {
            var time = DateTime.Now;

            var buffer = new byte[5];

            var value = new BytesStoreValue(time, buffer, 1, 2);

            Assert.AreEqual(value.Value, buffer);
            Assert.AreEqual(value.Offset, 1);
            Assert.AreEqual(value.Length, 2);
            Assert.AreEqual(value.Time, time);
        }

        [TestMethod]
        public void InitWithNullValue_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BytesStoreValue(default, null, 0, 0));
        }

        [TestMethod]
        public void InitWithOutOfRange_MustThrowArgumentOutOfRangeException()
        {
            var bytes = new byte[1];

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BytesStoreValue(default, bytes, 1, 1));
        }

        [TestMethod]
        public void ToStringWillBeSame()
        {
            BytesStoreValue value = "test";
            Assert.AreEqual(value.ToString(), $"{{{value.Time:o}}} {Encoding.UTF8.GetString(value.Value, value.Offset, value.Length)}}}");
        }
    }
}
