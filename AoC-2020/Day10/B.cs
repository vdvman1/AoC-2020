using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day10
{
    public class B
    {
        public static async Task Run()
        {
            List<int> numbers = (await A.Load()).ToList();

            int device = numbers.Max() + 3;

            // Use dynamic programming to avoid recounting the number of combinations starting from a given number multiple times
            var counts = new Dictionary<int, long>();

            long count = 0;
            for (int i = 0; i < numbers.Count && numbers[i] < 4; i++)
            {
                count += CountCombinations(numbers, i, counts, device, Array.Empty<int>());
            }

            Console.WriteLine($"Number of combinations: {count}");
        }

        private static long CountCombinations(List<int> numbers, int startIndex, Dictionary<int, long> counts, int device, IEnumerable<int> currentNumbers)
        {
            int start = numbers[startIndex];
            currentNumbers = currentNumbers.Append(start);

            if (startIndex + 1 >= numbers.Count)
            {
                Console.WriteLine($"(0), {string.Join(", ", currentNumbers)}, ({device})");
                return 1;
            }

            if(counts.TryGetValue(start, out long count))
            {
                Console.WriteLine($"(0), {string.Join(", ", currentNumbers)}, ..., ({device})");
                return count;
            }

            count = 0;
            for (int i = startIndex + 1; i < numbers.Count && numbers[i] - start < 4; i++)
            {
                count += CountCombinations(numbers, i, counts, device, currentNumbers);
            }

            counts.Add(start, count);
            return count;
        }
    }
}
