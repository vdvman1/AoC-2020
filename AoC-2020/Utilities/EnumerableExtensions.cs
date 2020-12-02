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
    }
}
