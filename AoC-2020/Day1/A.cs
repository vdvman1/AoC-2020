using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day1
{
    public class A
    {
        public static async Task Run()
        {
            var numbers = await Load();
            bool found = false;
            while (numbers.Count >= 2)
            {
                int a = numbers.First();
                int b = 2020 - a;
                numbers.Remove(a);
                if (numbers.Remove(b))
                {
                    Console.WriteLine($"Pair found: {a} + {b} = 2020, {a} * {b} = {a * b}");
                    found = true;
                }
            }

            if(!found)
            {
                Console.WriteLine("No pairs that sum to 2020 were found");
            }
        }

        public static async Task<HashSet<int>> Load() => new HashSet<int>(await Loading.LoadNumbers(nameof(Day1), signed: true));
    }
}
