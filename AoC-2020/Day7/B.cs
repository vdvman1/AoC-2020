using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day7
{
    public class B
    {
        public async Task Run()
        {
            (Dictionary<string, List<(int count, string name)>> children, _) = await A.ParseBagFile();

            try
            {
                int count = CountChildren("shiny gold", children);
                Console.WriteLine($"Number of bags inside the 'shiny gold' bag: {count}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static int CountChildren(string bag, Dictionary<string, List<(int count, string name)>> children)
        {
            // TODO: cache intermediary results
            int Count((int count, string name) bag, IEnumerable<string> visited)
            {
                if(visited.Contains(bag.name))
                {
                    throw new InvalidOperationException($"Inifinitely many bags required! {bag.name}");
                }

                visited = visited.Prepend(bag.name);
                return children.TryGetValue(bag.name, out List<(int count, string name)>? values) ? bag.count * (values.Select(b => Count(b, visited)).Sum() + 1) : bag.count;
            }

            var visited = new[] { bag };
            return children.TryGetValue(bag, out List<(int count, string name)>? values) ? values.Select(b => Count(b, visited)).Sum() : 0;
        }
    }
}
