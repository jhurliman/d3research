using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.AI
{
    internal struct HeapEntry
    {
        public AITask Item;
        public IComparable Priority;

        public HeapEntry(AITask item, IComparable priority)
        {
            Item = item;
            Priority = priority;
        }

        public void Clear()
        {
            Item = null;
            Priority = null;
        }
    }

    public class AITaskQueue
    {
        private int count;
        private int capacity;
        private int version;
        private HeapEntry[] heap;

        public AITaskQueue()
        {
            capacity = 15; // 15 is equal to 4 complete levels
            heap = new HeapEntry[capacity];
        }

        public int Count { get { return count; } }

        public AITask Dequeue()
        {
            if (count == 0)
                throw new InvalidOperationException();

            var result = heap[0].Item;
            count--;
            trickleDown(0, heap[count]);
            heap[count].Clear();
            version++;
            return result;
        }

        public void Enqueue(AITask item, IComparable priority)
        {
            if (priority == null)
                throw new ArgumentNullException("priority");
            if (count == capacity)
                growHeap();
            count++;
            bubbleUp(count - 1, new HeapEntry(item, priority));
            version++;
        }

        private void bubbleUp(int index, HeapEntry he)
        {
            int parent = getParent(index);
            // note: (index > 0) means there is a parent
            while ((index > 0) &&
                  (heap[parent].Priority.CompareTo(he.Priority) < 0))
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
            HeapEntry[] newHeap = new HeapEntry[capacity];
            System.Array.Copy(heap, 0, newHeap, 0, count);
            heap = newHeap;
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
