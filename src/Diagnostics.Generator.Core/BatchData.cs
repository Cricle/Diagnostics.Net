using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Diagnostics.Generator.Core
{
    public readonly struct BatchData<T> : IEnumerable<T>, IDisposable
    {
        internal readonly T[] datas;
        internal readonly int count;

        internal BatchData(T[] datas, int count)
        {
            Debug.Assert(datas != null);

            this.datas = datas!;
            this.count = count;
        }

        public Span<T> Datas => datas == null ? Span<T>.Empty : datas.AsSpan(0, count);

        public int Count => count;

        public void Dispose()
        {
            if (datas != null)
            {
                ArrayPool<T>.Shared.Return(datas);
            }
        }

        public T[] DangerousGetDatas()
        {
            return datas;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (datas == null)
            {
                return Enumerable.Empty<T>().GetEnumerator();
            }
            return datas.Take(count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
