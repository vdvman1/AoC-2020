using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day6
{
    public class B
    {
        public static async Task Run()
        {
            string[] lines = await Loading.Load(nameof(Day6));
            int count = 0;
            for (int l = 0; l < lines.Length; l++)
            {
                if (string.IsNullOrWhiteSpace(lines[l])) continue;

                var yesAnswers = new HashSet<char>(lines[l]);
                l++;

                while (l < lines.Length && !string.IsNullOrWhiteSpace(lines[l]))
                {
                    yesAnswers.IntersectWith(lines[l]);
                    l++;
                }
                count += yesAnswers.Count;
            }
            Console.WriteLine($"Total number of shared yes answers: {count}");
        }
    }
}
