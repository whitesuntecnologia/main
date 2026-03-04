using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticClass.Extensions
{
    public static class SqlStyleExtensions
    {
        public static bool In<T>(this T me, params T[] set)
        {
            return set.Contains(me);
        }

        public static bool NotIn<T>(this T me, params T[] set)
        {
            return !set.Contains(me);
        }
    }
}
