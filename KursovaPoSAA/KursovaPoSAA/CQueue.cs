using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovaPoSAA
{
    public class CQueue<T> : Container<T>
    {
        public CQueue() : base() { }

        public CQueue(IEnumerable<T> collection) : this()
        {
            foreach (var item in collection)
            {
                Enqueue(item);
            }
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty!");
            }

            //Retrieve the Value at the Front:
            var value = Begin.Next.Value;


            //Adjust Pointers to Remove the Front Element:
            Begin.Next = Begin.Next.Next;
            Begin.Next.Prev = Begin;
            Count--;

            return value;
        }
        public void Enqueue(T value)
        {
            var item = new Item<T>()
            {
                Prev = End.Prev,
                Next = End,
                Value = value
            };

            //Adjust Pointers to Insert the New Item:
            End.Prev.Next = item;
            End.Prev = item;
            Count++;
        }
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty!");
            }

            return Begin.Next.Value;
        }
    }
}
