namespace Diagnostics.Traces.Serialization
{
    public unsafe struct ConstMiniReadSerializer : IMiniReadSerializer
    {
        private int offset;
        private readonly byte* buffer;
        private readonly int bufferLength;


        public ConstMiniReadSerializer(byte* buffer, int bufferLength)
        {
            if (buffer == null)
            {
                throw new ArgumentException("The buffer is nullptr");
            }

            if (bufferLength <= 0)
            {
                throw new ArgumentException("The buffer length is min or equals than zero");
            }

            this.buffer = buffer;
            this.bufferLength = bufferLength;
        }

        public int BuffetLength => bufferLength;

        public int Offset => offset;

        public bool CanSeek => true;

        public bool CanRead(int length)
        {
            if (length < 0)
            {
                throw new ArgumentException($"The length must more than zero");
            }
            return (bufferLength - offset) >= length;
        }

        public void Skip(int length)
        {
            if (!CanRead(length))
            {
                throw new ArgumentOutOfRangeException($"The total offset is {bufferLength - length} can't move {length}");
            }
            offset += length;
        }

        public void Read(Span<byte> buffer)
        {
            if (bufferLength >= buffer.Length)
            {
                new Span<byte>((this.buffer + offset), buffer.Length).CopyTo(buffer);
                offset += buffer.Length;
                return;
            }
            throw new ArgumentOutOfRangeException("buffer", $"The buffer size is {bufferLength}, but the buffer is {buffer.Length}");
        }
    }
}
