using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public static class EnumerableExtensions
    {
        public static string JoinWithLast<T>(this IEnumerable<T> values, string sep, string? finalSep = null)
        {
            if (finalSep is null) return string.Join(sep, values);

            using var enumerator = values.GetEnumerator();
            if (!enumerator.MoveNext()) return string.Empty;

            string? current = enumerator.Current?.ToString();
            if(!enumerator.MoveNext())
            {
                return current ?? string.Empty;
            }

            var builder = new StringBuilder(current);
            current = enumerator.Current?.ToString();
            while(enumerator.MoveNext())
            {
                builder.Append(sep);
                builder.Append(current);
                current = enumerator.Current?.ToString();
            }

            builder.Append(finalSep);
            builder.Append(current);

            return builder.ToString();
        }

        public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> values)
        {
            using var it = values.GetEnumerator();
            if (!it.MoveNext()) yield break;

            T prev = it.Current;
            while (it.MoveNext())
            {
                T next = it.Current;
                yield return (prev, next);
                prev = next;
            }
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values)
            where T : struct
            => values.Where(v => v.HasValue).Select(v => v!.Value);

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values) => (IEnumerable<T>)values.Where(v => v is not null);
    }
}
