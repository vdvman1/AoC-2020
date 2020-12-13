using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day9
{
    public class A
    {
        public static async Task Run()
        {
            IEnumerable<int> data = await Load();

            if(FindBrokenNumber(data) is int value)
            {
                Console.WriteLine($"The number {value} is not the sum of 2 of the past 25 numbers");
            }
            else
            {
                Console.WriteLine($"No broken number found");
            }
        }

        public static Task<IEnumerable<int>> Load() => Loading.LoadNumbers(nameof(Day9));

        public static int? FindBrokenNumber(IEnumerable<int> data)
        {
            const int preambleLength = 25;
            var numbers = new CircularBuffer<int>(preambleLength);

            IEnumerator<int> it = data.GetEnumerator();
            while (numbers.Count < preambleLength)
            {
                if (!it.MoveNext())
                {
                    Console.WriteLine("Not enough numbers");
                    return null;
                }

                numbers.Add(it.Current);
            }

            while (it.MoveNext())
            {
                int value = it.Current;
                bool missing = true;
                for (int i = 0; missing && i < preambleLength; i++)
                {
                    int a = numbers[i];
                    int b = value - a;
                    for (int j = i + 1; j < preambleLength; j++)
                    {
                        if (numbers[j] == b)
                        {
                            missing = false;
                            break;
                        }
                    }
                }

                if (missing)
                {
                    return value;
                }

                numbers.Add(value);
            }

            return null;
        }
    }
}
