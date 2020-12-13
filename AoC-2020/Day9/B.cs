using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day9
{
    public class B
    {
        public static async Task Run()
        {
            List<int> data = (await A.Load()).ToList();

            if(A.FindBrokenNumber(data) is not int value)
            {
                Console.WriteLine("No number found");
                return;
            }

            if(FindWeakness(data, value) is int weakness)
            {
                Console.WriteLine($"Weakness: {weakness}");
            }
            else
            {
                Console.WriteLine("No weakness found");
            }
        }

        private static int? FindWeakness(List<int> data, int value)
        {
            for (int i = 0; i < data.Count; i++)
            {
                int smallest = data[i];
                if (smallest == value)
                {
                    return smallest + smallest;
                }
                int largest = smallest;
                int total = smallest;
                for (int j = i + 1; total < value && j < data.Count; j++)
                {
                    int v = data[j];
                    if (v < smallest)
                    {
                        smallest = v;
                    }

                    if (v > largest)
                    {
                        largest = v;
                    }

                    total += v;

                    if(total == value)
                    {
                        return smallest + largest;
                    }
                }
            }

            return null;
        }
    }
}
