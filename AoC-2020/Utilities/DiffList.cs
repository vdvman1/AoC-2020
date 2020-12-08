using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public class DiffList<T> : IList<T>
    {
        private readonly IReadOnlyList<T> List;
        private readonly Dictionary<int, T> Diffs = new();

        // TODO: Implement insertions and removals

        public DiffList(IReadOnlyList<T> list)
        {
            List = list;
            Count = list.Count;
        }

        public T this[int index]
        {
            get => Diffs.TryGetValue(index, out T? value) ? value : List[index];
            set
            {
                if ((uint)index < (uint)Count)
                {
                    Diffs.Add(index, value);
                }
                else
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Diffs.Add(Count, item);
            Count++;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            if(Diffs.ContainsValue(item))
            {
                return true;
            }

            for (int i = 0; i < List.Count; i++)
            {
                if (Diffs.ContainsKey(i)) continue;

                if(item is null)
                {
                    if(List[i] is null)
                    {
                        return true;
                    }
                }
                else if(item.Equals(List[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex + Count > array.Length) throw new IndexOutOfRangeException();

            int i = 0;

            for (; i < List.Count; i++)
            {
                array[i + arrayIndex] = Diffs.TryGetValue(i, out T? value) ? value : List[i];
            }

            for(; i < Count; i++)
            {
                array[i + arrayIndex] = GetDiff(i);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            for (; i < List.Count; i++)
            {
                if(Diffs.TryGetValue(i, out T? value))
                {
                    yield return value;
                }
                else
                {
                    yield return List[i];
                }
            }

            for(; i < Count; i++)
            {
                yield return GetDiff(i);
            }
        }

        private T GetDiff(int i)
        {
            if (!Diffs.TryGetValue(i, out T? value))
            {
                Trace.Assert(false, $"{nameof(Count)} > {nameof(List)}.{nameof(List.Count)} must have all indices greater than the list length filled");
                throw new IndexOutOfRangeException(nameof(Count));
            }

            return value;
        }

        public int IndexOf(T item)
        {
            foreach (KeyValuePair<int, T> pair in Diffs)
            {
                if(item is null)
                {
                    if(pair.Value is null)
                    {
                        return pair.Key;
                    }
                }
                else if(item.Equals(pair.Value))
                {
                    return pair.Key;
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                if (Diffs.ContainsKey(i)) continue;

                if (item is null)
                {
                    if (List[i] is null)
                    {
                        return i;
                    }
                }
                else if (item.Equals(List[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
