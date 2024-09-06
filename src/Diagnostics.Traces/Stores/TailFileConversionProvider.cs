namespace Diagnostics.Traces.Stores
{
    public class TailFileConversionProvider : IFileConversionProvider
    {
        public TailFileConversionProvider(string tail)
        {
            Tail = tail ?? throw new ArgumentNullException(nameof(tail));
        }

        public string Tail { get; }

        public string ConvertPath(string filePath)
        {
            return filePath + Tail;
        }
    }
}
