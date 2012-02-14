using System;
using System.Collections.Generic;

namespace libdiablo3
{
    internal class PriorityQueue<T>
    {
        private struct HeapEntry
        {
            public T Item;
            public IComparable Priority;

            public HeapEntry(T item, IComparable priority)
            {
                Item = item;
                Priority = priority;
            }

            public void Clear()
            {
                Item = default(T);
                Priority = null;
            }
        }

        private int count;
        private int capacity;
        private HeapEntry[] heap;

        public int Count { get { return count; } }

        public PriorityQueue()
        {
            capacity = 15; // 15 is equal to 4 complete levels
            heap = new HeapEntry[capacity];
        }

        public void Clear()
        {
            heap = new HeapEntry[capacity];
        }

        public T Dequeue()
        {
            if (count == 0)
                throw new InvalidOperationException();

            var result = heap[0].Item;
            count--;
            trickleDown(0, heap[count]);
            heap[count].Clear();
            return result;
        }

        public void Enqueue(T item, IComparable priority)
        {
            if (priority == null)
                throw new ArgumentNullException("priority");
            if (count == capacity)
                growHeap();
            count++;
            bubbleUp(count - 1, new HeapEntry(item, priority));
        }

        private void bubbleUp(int index, HeapEntry he)
        {
            int parent = getParent(index);
            while ((index > 0) && // note: (index > 0) means there is a parent
                    heap[parent].Priority.CompareTo(he.Priority) < 0)
            {
                heap[index] = heap[parent];
                index = parent;
                parent = getParent(index);
            }
            heap[index] = he;
        }

        private int getLeftChild(int index)
        {
            return (index * 2) + 1;
        }

        private int getParent(int index)
        {
            return (index - 1) / 2;
        }

        private void growHeap()
        {
            capacity = (capacity * 2) + 1;
            Array.Resize(ref heap, capacity);
        }

        private void trickleDown(int index, HeapEntry he)
        {
            int child = getLeftChild(index);
            while (child < count)
            {
                if (((child + 1) < count) &&
                    (heap[child].Priority.CompareTo(heap[child + 1].Priority) < 0))
                {
                    child++;
                }
                heap[index] = heap[child];
                index = child;
                child = getLeftChild(index);
            }
            bubbleUp(index, he);
        }
    }
}
