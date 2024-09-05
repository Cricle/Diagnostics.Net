namespace Diagnostics.Traces
{
    public abstract class BytesStoreBase : IBytesStore
    {
        public abstract string Name { get; }

        public abstract int Count();
        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public abstract void Dispose();
        public abstract void Insert(BytesStoreValue value);
        public virtual Task InsertAsync(BytesStoreValue value)
        {
            Insert(value);
            return Task.CompletedTask;
        }
        public abstract void InsertMany(IEnumerable<BytesStoreValue> strings);
        public virtual Task InsertManyAsync(IEnumerable<BytesStoreValue> strings)
        {
            InsertMany(strings);
            return Task.CompletedTask;
        }
    }
}
