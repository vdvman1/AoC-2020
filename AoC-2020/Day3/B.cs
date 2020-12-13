using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day3
{
    public class B
    {
        public static async Task Run()
        {
            (bool[][] treeGrid, int width) = await A.Load();
            long product = 1;
            foreach ((int xStep, int yStep) in new[] {
                (1, 1), (3, 1), (5, 1), (7, 1), (1, 2)
            })
            {
                int count = A.CountTrees(treeGrid, width, xStep, yStep);
                product *= count;
                Console.WriteLine($"Trees intercepted on slope ({xStep}, {yStep}): {count}, partial product: {product}");
            }

            Console.WriteLine($"Product of trees intercepted: {product}");
        }
    }
}
