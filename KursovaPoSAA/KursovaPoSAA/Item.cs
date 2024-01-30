using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovaPoSAA
{
    class Item<T>
    {
        internal Item<T> Prev;
        internal Item<T> Next;

        internal T Value;

        internal void Clear()
        {
            Prev = null;
            Next = null;
            Value = default;
        }
    }
}
