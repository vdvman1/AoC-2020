using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> Flatten<T>(this T[,] values)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    yield return values[i, j];
                }
            }
        }
    }
}
