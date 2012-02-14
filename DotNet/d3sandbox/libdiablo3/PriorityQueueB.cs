using System;
using System.Collections.Generic;

namespace libdiablo3
{
    internal class PriorityQueueB<T>
    {
        private int count;
        private int capacity;
        private T[] heap;
        private IComparer<T> comparer;

        public int Count { get { return count; } }

        public PriorityQueueB(IComparer<T> comparer)
        {
            capacity = 15; // 15 is equal to 4 complete levels
            heap = new T[capacity];
            this.comparer = comparer;
        }

        public void Clear()
        {
            heap = new T[capacity];
        }

        public T Dequeue()
        {
            if (count == 0)
                throw new InvalidOperationException();

            var result = heap[0];
            count--;
            trickleDown(0, heap[count]);
            heap[count] = default(T);
            return result;
        }

        public void Enqueue(T item)
        {
            if (count == capacity)
                growHeap();
            count++;
            bubbleUp(count - 1, item);
        }

        private void bubbleUp(int index, T he)
        {
            int parent = getParent(index);
            // note: (index > 0) means there is a parent
            while (index > 0 && comparer.Compare(heap[parent], he) < 0)
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

        private void trickleDown(int index, T he)
        {
            int child = getLeftChild(index);
            while (child < count)
            {
                if (child + 1 < count && comparer.Compare(heap[child], heap[child + 1]) < 0)
                    child++;
                heap[index] = heap[child];
                index = child;
                child = getLeftChild(index);
            }
            bubbleUp(index, he);
        }
    }
}
