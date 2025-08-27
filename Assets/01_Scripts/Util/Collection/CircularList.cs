using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Util.Logger;

namespace Util.Collection {
    [Serializable]
    public class CircularList<T> : IEnumerable<T> where T : class {
        int index = 0;
        List<T> list;

        public int Count => list.Count;
        public int Pivot => index;
        public bool IsAtFirst => index == 0;
        public bool IsAtLast => index == list.Count - 1 && list.Count > 0;
        public bool IsEmpty => list.Count == 0;
        public T Current => (list.Count > 0) ? list[index] : null;
        public List<T> Items => list;

        public override string ToString() =>
            $"[Circular<{typeof(T).Name}>] (Current Index: {index})\n" +
            $"{string.Join(",\n ", list.Select((item, i) => $"{i}. {item}"))}";

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public CircularList() {
            index = 0;
            list = new();
        }
        public CircularList(int pivot, IEnumerable<T> list) {
            index = pivot;
            this.list = new(list);
        }
        public CircularList(CircularList<T> list) {
            index = list.index;
            this.list = new(list.Items);
        }
        public CircularList(int pivot, int size) {
            index = pivot;
            this.list = new(size);
        }
        public CircularList(int size) {
            index = 0;
            this.list = new(size);
        }

        public void Add(T item) => list.Add(item);
        public void AddRange(IEnumerable<T> items) => list.AddRange(items);


        public void RemoveCurrent() {
            if (list.Count == 0) return;
            list.RemoveAt(index);
            if (index >= list.Count) index = 0;
        }

        public void RemoveAt(int index) {
            if (index < 0 || index > list.Count - 1) return;
            list.RemoveAt(index);
            if (index >= list.Count) this.index = 0;
        }

        public void Remove(T item) {
            int idx = list.IndexOf(item);
            if (idx >= 0)
                RemoveAt(idx);
        }


        public void MoveToFirst() {
            index = 0;
        }

        public void MoveNext() {
            if (list.Count == 0) return;
            index = (index + 1) % list.Count;
        }

        public void MoveToLast() {
            if (list.Count == 0) return;
            index = list.Count - 1;
        }

        public void MoveTo(int index) {
            if (index < 0 && index > list.Count - 1) {
                HLogger.Exception(new IndexOutOfRangeException(), $"Input index is '{index}'");
                return;
            }
            this.index = index;
        }

        public void MoveTo(T target) {
            int pivot = list.IndexOf(target);
            if (pivot >= 0) index = pivot;
        }


        public void Clear() {
            list.Clear();
            index = 0;
        }
    }
}


/* @Jason
 * 16. May. 2025.
 * 1. Create class.
 * 29. May. 2025
 * 1. Implement 'IEnumerable'.
 * 2. Edit 'ToString' format.
 * 3. Add pivot position boolean properties.
 * 4. Add constructors.
 * 24. Jun. 2025
 * 1. Add extra constructors.
 */