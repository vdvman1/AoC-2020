using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day5
{
    public class B
    {
        public async Task Run()
        {
            string[] lines = await File.ReadAllLinesAsync(Path.Combine("Day5", "input.txt"));
            IEnumerable<(int, int)> ids = lines.Select(A.ToId).Where(id => id.HasValue).Select(id => id!.Value).OrderBy(id => id).Pairwise();
            foreach ((int a, int b) in ids)
            {
                if(a + 1 != b)
                {
                    Console.WriteLine($"Boarding seat id: {a + 1}");
                    return;
                }
            }
        }
    }
}
