using Microsoft.Extensions.ObjectPool;
using System.Buffers;

namespace NativeBuffering
{
    internal sealed class PooledList<T>
    {
        private static readonly ObjectPool<PooledList<T>> _pool = new DefaultObjectPoolProvider().Create<PooledList<T>>(new PooledListPolicy());
        private T[] _array = null!;
        private int _index = 0;

        public int Count => _index;
        private PooledList() { }

        public void Initialize(int count)
        {
            _array = ArrayPool<T>.Shared.Rent(count);
            _index = 0;
        }

        public void Release()
        {
            _index = 0;
            if (_array is not null)
            {
                ArrayPool<T>.Shared.Return(_array);
            }
        }

        public void Add(T item)
        {
            if (_index < _array.Length)
            {
                _array[_index++] = item;
                return;
            }

            var array = ArrayPool<T>.Shared.Rent(_array.Length * 2);
            _array.CopyTo(array, 0);
            ArrayPool<T>.Shared.Return(_array, clearArray: true);
            _array = array;
            _array[_index++] = item;
        }   

        public ArraySegment<T> ToArraySegment() => new(_array, 0, _index);

        public static PooledList<T> Rent(int capacity)
        { 
            var list = _pool.Get();
            list.Initialize(capacity);
            return list;
        }       

        public void Return() => _pool.Return(this);
        
        private sealed class PooledListPolicy: IPooledObjectPolicy<PooledList<T>>
        {
            public PooledList<T> Create() => new();
            public bool Return(PooledList<T> obj) { obj.Release(); return true; }
        }
    }
}
