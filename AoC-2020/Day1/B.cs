using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day1
{
    public class B
    {
        public static async Task Run()
        {
            var numbers = await A.Load();
            var triplets = new HashSet<(int a, int b, int c)>();
            bool found = false;
            while (numbers.Count >= 3)
            {
                int a = numbers.First();
                numbers.Remove(a);
                foreach (var b in numbers)
                {
                    int c = 2020 - a - b;
                    if (
                        numbers.Contains(c)
                        && !triplets.Contains((a, c, b)) // Checking the reverse for b and c as order does not matter
                        && triplets.Add((a, b, c))
                    )
                    {
                        Console.WriteLine($"Triplet found: {a} + {b} + {c} = 2020, {a} * {b} * {c} = {a * b * c}");
                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("No pairs that sum to 2020 were found");
            }
        }
    }
}
