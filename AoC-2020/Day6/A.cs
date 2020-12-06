using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day6
{
    public class A
    {
        public async Task Run()
        {
            string[] lines = await File.ReadAllLinesAsync(Path.Combine("Day6", "input.txt"));
            int count = 0;
            for (int l = 0; l < lines.Length; l++)
            {
                var yesAnswers = new HashSet<char>();
                while (l < lines.Length && !string.IsNullOrWhiteSpace(lines[l]))
                {
                    foreach (char c in lines[l])
                    {
                        yesAnswers.Add(c);
                    }
                    l++;
                }
                count += yesAnswers.Count;
            }
            Console.WriteLine($"Total number of unique yes answers: {count}");
        }
    }
}
