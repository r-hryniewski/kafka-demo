using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public class CircullarBuffer<T> : IReadOnlyCollection<T>, IEnumerable<T>
    {
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly uint capacity;

        public bool IsFull => Count >= capacity;

        public CircullarBuffer(uint capacity)
        {
            this.capacity = capacity;
        }

        public void Add(T item)
        {
            if (IsFull)
            {
                queue.TryDequeue(out var _);
            }

            queue.Enqueue(item);
        }

        public int Count => queue.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IReadOnlyCollection<T>)queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyCollection<T>)queue).GetEnumerator();
        }
    }
}
