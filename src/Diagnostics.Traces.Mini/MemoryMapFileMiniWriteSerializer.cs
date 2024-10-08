﻿using Diagnostics.Traces.Serialization;
using System.Runtime.CompilerServices;

namespace Diagnostics.Traces.Mini
{
    public class MemoryMapFileMiniWriteSerializer : BufferMiniWriteSerializer
    {
        private readonly MemoryMapFileManger memoryMapFileManger;

        public long Writed => memoryMapFileManger.Writed;

        public MiniWriteTraceHelper TraceHelper { get; }

        public MemoryMapFileMiniWriteSerializer(string filePath, long capacity,bool autoCapacity)
        {
            memoryMapFileManger = new MemoryMapFileManger(filePath, capacity, autoCapacity);
            memoryMapFileManger.Seek(TraceHeader.HeaderSize, SeekOrigin.Begin);
            TraceHelper = new MiniWriteTraceHelper(this);
        }

        public override bool CanWrite(int length)
        {
            return memoryMapFileManger.CanWrite(length);
        }
        protected override void WriteCore(ReadOnlySpan<byte> buffer)
        {
            memoryMapFileManger.Write(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteHead(TraceHeader header)
        {
            byte* buffer = stackalloc byte[TraceHeader.HeaderSize];
            Unsafe.Write(buffer, header);
            memoryMapFileManger.WriteHead(new ReadOnlySpan<byte>(buffer, TraceHeader.HeaderSize));
        }

        public void Seek(int offset, SeekOrigin origin)
        {
            memoryMapFileManger.Seek(offset, origin);
        }
        protected override void OnDisposed()
        {
            memoryMapFileManger.Dispose();
        }
    }
}
