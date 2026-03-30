using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeybindManager
{
    public static class Extensions
    {
        public static DistinctList<T> ToOrderedCollection<T>(this IEnumerable<T> collection)
        {
            return new DistinctList<T>(collection);
        }
    }
}
