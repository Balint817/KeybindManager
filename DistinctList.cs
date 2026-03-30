using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace KeybindManager
{
    public class DistinctList<T> : IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly HashSet<T> _set = new HashSet<T>();

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
        }

        public DistinctList()
        {

        }
        internal DistinctList(IEnumerable<T> values)
        {
            if (values is null) return;

            foreach (var item in values)
            {
                if (_set.Add(item))
                {
                    _list.Add(item);
                }
            }
        }
        public int Count => _list.Count;
        public bool Add(T item)
        {
            if (!_set.Add(item))
            {
                return false;
            }

            _list.Add(item);
            return true;
        }

        public bool Insert(int index, T item)
        {
            if (!_set.Add(item))
            {
                return false;
            }
            try
            {
                _list.Insert(index, item);
            }
            finally
            {
                _set.Remove(item);
            }
            return true;
        }

        public void Switch(int index1, int index2)
        {
            if (index1 < 0 || index1 >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index1));
            }
            if (index2 < 0 || index2 >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index2));
            }
            if (index1 == index2) return;

            var temp = _list[index1];
            _list[index1] = _list[index2];
            _list[index2] = _list[index1];
        }

        public void Clear()
        {
            _set.Clear();
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex = 0)
        {
            foreach (var item in _list)
            {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item)
        {
            _list.Remove(item);
            return _set.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            _set.Remove(item);
        }

        private void RangeChecks(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if ((index + count) > _list.Count)
            {
                throw new ArgumentException($"{nameof(index)} and {nameof(count)} do not denote a valid range of elements");
            }

        }
        public IEnumerable<T> GetRange(int index, int count)
        {
            RangeChecks(index, count);
            for (int i = index; i < index + count; i++)
            {
                yield return _list[i];
            }
        }

        public void RemoveRange(int index, int count)
        {
            RangeChecks(index, count);
            for (int i = 0; i < count; i++)
            {
                var item = _list[index];
                _list.RemoveAt(index);
                _set.Remove(item);
            }
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return _list.AsReadOnly();
        }
        public void Sort()
        {
            _list.Sort();
        }
        public void Sort(IComparer<T> comparer)
        {
            _list.Sort(comparer);
        }
        public void Sort(Comparison<T> comparer)
        {
            _list.Sort(comparer);
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _list.Sort(index, count, comparer);
        }
    }

}
