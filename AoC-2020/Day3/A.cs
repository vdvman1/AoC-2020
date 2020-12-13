using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day3
{
    public class A
    {
        public static async Task<(bool[][] treeGrid, int width)> Load()
        {
            string[] lines = await Loading.Load(nameof(Day3));
            bool[][] treeGrid = lines.Select(line => line.Select(c => c == '#').ToArray()).ToArray();
            int width = treeGrid.Select(line => line.Length).Distinct().Single();
            return (treeGrid, width);
        }

        public static int CountTrees(bool[][] treeGrid, int width, int xStep, int yStep)
        {
            int treeCount = 0;
            int x = 0;
            for (int y = 0; y < treeGrid.Length; y += yStep)
            {
                if (treeGrid[y][x])
                {
                    treeCount++;
                }

                x += xStep;
                while (x >= width)
                {
                    x -= width;
                }
            }

            return treeCount;
        }

        public static async Task Run()
        {
            (bool[][] treeGrid, int width) = await Load();

            Console.WriteLine($"Trees intercepted: {CountTrees(treeGrid, width, 3, 1)}");
        }
    }
}
