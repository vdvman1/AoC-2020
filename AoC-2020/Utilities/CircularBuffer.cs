using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] Items;
        private int Pos = 0;

        public int Count { get; private set; } = 0;

        public int Capacity => Items.Length;

        public CircularBuffer(int size) => Items = new T[size];

        public void Add(T value)
        {
            if(Count == Items.Length)
            {
                Items[Pos] = value;
                Pos = AddWrap(Pos, 1);
            }
            else
            {
                Items[AddWrap(Pos, Count)] = value;
                Count++;
            }
        }

        public T this[int i]
        {
            get => (uint)i < (uint)Count ? Items[AddWrap(Pos, i)] : throw new IndexOutOfRangeException();
            set
            {
                if((uint)i < (uint)Count)
                {
                    Items[AddWrap(Pos, i)] = value;
                }
                
                throw new IndexOutOfRangeException();
            }
        }

        private int AddWrap(int value, int amount) => (value + amount) % Items.Length;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return Items[AddWrap(Pos, i)];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
