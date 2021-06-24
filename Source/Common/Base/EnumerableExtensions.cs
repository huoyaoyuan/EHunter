using System.Collections.Generic;

namespace EHunter
{
    public static class EnumerableExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> value)
        {
            if (list is List<T> l)
            {
                l.AddRange(value);
            }
            else
            {
                foreach (var v in value)
                    list.Add(v);
            }
        }
    }
}
