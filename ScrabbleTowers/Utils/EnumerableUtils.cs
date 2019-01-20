using System;
using System.Collections.Generic;

namespace ScrabbleTowers.Utils
{
    public static class EnumerableUtils
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> source)
        {
            while (source.MoveNext()) {
                yield return source.Current;
            }
        }
    }
}