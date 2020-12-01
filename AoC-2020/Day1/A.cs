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
        public async Task Run()
        {
            string[] lines = await File.ReadAllLinesAsync(Path.Combine("Day1", "input.txt"));
            HashSet<int> numbers = new(lines.AsParallel().Select(l => (int.TryParse(l, out int num), num)).Where(l => l.Item1).Select(l => l.num));
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
    }
}
