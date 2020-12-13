using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day7
{
    public class A
    {
        public static async Task Run()
        {
            (_, Dictionary<string, List<string>> parents) = await ParseBagFile();

            int count = CountUniqueParents("shiny gold", parents);
            Console.WriteLine($"Number of bags that can (in)directly hold 'shiny gold' bags: {count}");
        }

        public static async Task<(Dictionary<string, List<(int count, string name)>> children, Dictionary<string, List<string>> parents)> ParseBagFile()
        {
            string[] lines = await Loading.Load(nameof(Day7));
            var children = new Dictionary<string, List<(int count, string name)>>();
            var parents = new Dictionary<string, List<string>>();
            foreach (string line in lines)
            {
                try
                {
                    var parser = new Parser(line);
                    parser.Required(parser.AnyUntil(out string parentName, " bags contain "));
                    if (!children.TryGetValue(parentName, out List<(int count, string name)>? childNodes))
                    {
                        childNodes = new();
                        children.Add(parentName, childNodes);
                    }

                    if(parser.Remainder == "no other bags.")
                    {
                        continue;
                    }

                    int delim;
                    do
                    {
                        parser.Optional(parser.Whitespace());
                        parser.Required(parser.Integer(out int count));
                        parser.Required(parser.AnyUntilOneOf(out string child, out delim, ',', '.'));
                        child = child.Trim();
                        string bag = count == 1 ? "bag" : "bags";
                        if (!child.EndsWith(bag))
                        {
                            Console.WriteLine($"Invalid bag spec '{line}': expected {bag}");
                            break;
                        }

                        string childName = child[0..^bag.Length].Trim();
                        childNodes.Add((count, childName));

                        if (!parents.TryGetValue(childName, out List<string>? parentNodes))
                        {
                            parentNodes = new();
                            parents.Add(childName, parentNodes);
                        }
                        parentNodes.Add(parentName);
                    } while (delim == 0);
                }
                catch (ParseException e)
                {
                    Console.WriteLine($"Invalid bag spec '{line}': {e.Message}");
                }
            }

            return (children, parents);
        }

        private static int CountUniqueParents(string bag, Dictionary<string, List<string>> parents)
        {
            HashSet<string> visited = new(){ bag };

            int Count(string bag)
            {
                if (!visited.Add(bag)) return 0;

                return parents.TryGetValue(bag, out List<string>? values) ? 1 + values.Select(Count).Sum() : 1;
            }

            return parents.TryGetValue(bag, out List<string>? values) ? values.Select(Count).Sum() : 0;
        }
    }
}
