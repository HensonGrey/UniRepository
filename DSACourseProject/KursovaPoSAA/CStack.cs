using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovaPoSAA
{
    public class CStack<T> : Container<T>
    {
        public CStack() : base() { }

        public CStack(IEnumerable<T> collection) : this()
        {
            foreach (var item in collection)
            {
                Push(item);
            }
        }

        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Stack is empty!");
            }

            return Begin.Next.Value;
        }

        public T Pop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Stack is empty!");
            }

            //Retrieve the Value at the Top of the Stack
            var value = Begin.Next.Value;

            //Adjust Pointers to Remove the Top Element:
            Begin.Next = Begin.Next.Next;
            Count--;

            return value;
        }

        public void Push(T value)
        {
            var item = new Item<T>
            {
                Next = Begin.Next,
                Value = value
            };

            //Adjust Pointers to Insert the New Item:
            Begin.Next = item;
            Count++;
        }
    }
}
