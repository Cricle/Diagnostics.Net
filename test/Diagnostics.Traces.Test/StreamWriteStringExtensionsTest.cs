namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class StreamWriteStringExtensionsTest
    {
        private string GetStreamString(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        [TestMethod]
        public void WriteString()
        {
            var mem = new MemoryStream();
            StreamWriteStringExtensions.WriteString(mem, "test");
            Assert.AreEqual(GetStreamString(mem), "test");
        }

        [TestMethod]
        public async Task WriteStringAsync()
        {
            var mem = new MemoryStream();
            await StreamWriteStringExtensions.WriteStringAsync(mem, "test");
            Assert.AreEqual(GetStreamString(mem), "test");
        }
    }
}
