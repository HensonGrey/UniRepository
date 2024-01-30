using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovaPoSAA
{
    public abstract class Container<T> : IEnumerable<T>
    {
        internal Item<T> Begin { get; }
        internal Item<T> End { get; }

        public int Count { get; protected set; }

        protected Container()
        {
            Begin = new Item<T>();
            End = new Item<T>();

            Begin.Next = End;
            End.Prev = Begin;
        }

        public virtual void Clear()
        {
            Begin.Clear();
            End.Clear();

            Begin.Next = End;
            End.Prev = Begin;

            Count = 0;
        }

        public bool Contains(T item)
        {
            foreach (var i in this)
            {
                if (EqualityComparer<T>.Default.Equals(i, item))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            var current = Begin.Next;

            while (current != End)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual T[] ToArray()
        {
            int index = 0;
            T[] tab = new T[Count];

            foreach (var item in this)
            {
                tab[index++] = item;
            }

            return tab;
        }
    }
}
