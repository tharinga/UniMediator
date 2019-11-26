using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace UniMediator.Internal
{
    /// <summary>
    /// Lazily allocates a List only when more than 1 item is added. Beware this
    /// is a Value Type and will behave in unexpected ways if you use it as you
    /// would a reference type, like a regular List
    /// </summary>
    /// <typeparam name="T">The type the elements in this List</typeparam>

    internal struct LazySingleItemList<T> 
    {
        private List<T> _list;
        private T _item0;
        private int _count;
        
        public int Count => _count;

        public void Add(T item)
        {
            if (_count == 0 && _list == null)
            {
                _item0 = item;
            }
            else if (_count == 1 && _list == null)
            {
                _list = new List<T>(4) { _item0, item };
            }
            else
            {
                _list.Add(item);
            }
            _count++;
        }

        public T this[int index]
        {
            get
            {
                if (index == 0 && _list == null) 
                    return _item0;
                return _list[index];
            }
        }

        /// <summary>
        /// Removes an item from the List by removing the last item and using it to 
        /// replace the item to be removed. Prevents array elements having to shift to
        /// the left, but does not maintain ordering
        /// </summary>
        /// <param name="item">The item to be removed</param>
        public void FastRemove(T item)
        {
            if (_count == 0) return;
            if (_list == null)
            {
                _item0 = default(T);
            }
            else
            {
                T last = GetAndRemoveLast();
                if (!EqualityComparer<T>.Default.Equals(last, item))
                {
                    _list[_list.IndexOf(item)] = last;
                }
            }
            _count--;
        }

        private T GetAndRemoveLast()
        {
            int lastIdx = _list.Count - 1;
            T last = _list[lastIdx];
            _list.RemoveAt(lastIdx);
            return last;
        }
    }
}