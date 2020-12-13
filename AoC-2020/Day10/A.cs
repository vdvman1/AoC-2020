using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day10
{
    public class A
    {
        public static async Task Run()
        {
            int ones = 0;
            int threes = 0;
            int prev = 0;
            foreach (int num in (await Load()).OrderBy(n => n))
            {
                switch(num - prev)
                {
                    case 1:
                        ones++;
                        break;
                    case 2:
                        break;
                    case 3:
                        threes++;
                        break;
                    case int diff:
                        Console.WriteLine($"Invalid difference between adapters rated '{prev}' and '{num}': {diff}");
                        return;
                }

                prev = num;
            }

            threes++; // Device is always 3 jolts higher than the largest adapter

            Console.WriteLine($"no. 1 jolt differences * no. 3 jolt differences = {(long)ones * threes}");
        }

        public static Task<IEnumerable<int>> Load() => Loading.LoadNumbers(nameof(Day10), signed: false);
    }
}
