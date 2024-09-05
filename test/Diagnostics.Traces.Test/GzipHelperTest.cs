using System.IO.Compression;
using System.Text;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class GzipHelperTest
    {
        [TestMethod]
        public void Compress_Empty()
        {
            var str = string.Empty;
            var res = GzipHelper.Compress(str);

            Assert.AreEqual(res.Count, 0);
            Assert.AreEqual(res.Result.Length, 0);
            Assert.AreEqual(res.Span.Length, 0);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Compress_WithEncoding(bool hasEncoding)
        {
            var str = "helloworld!";
            Encoding? encoding = hasEncoding ? Encoding.UTF8 : null;
            using var res = GzipHelper.Compress(str, encoding);

            var mem = new MemoryStream(res.Result, 0, res.Count);
            var dec = new StreamReader(new GZipStream(mem, CompressionMode.Decompress)).ReadToEnd();

            Assert.AreEqual(dec, str);
        }
    }
}
