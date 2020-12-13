using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public static class Loading
    {
        public static async Task<string[]> Load(string name) => await File.ReadAllLinesAsync(Path.Combine(name, "input.txt"));

        public static async Task<IEnumerable<int>> LoadNumbers(string name, bool signed)
        {
            string[] lines = await Load(name);
            return ((IEnumerable<string>)lines).Select(l => ParseNumber(l, signed)).WhereNotNull();
        }

        public static int? ParseNumber(string line, bool signed)
        {
            try
            {
                var parser = new Parser(line);
                int sign = 1;
                if(signed)
                {
                    // Automatically handle the default when not found being 0 by using 0 and -2 and just adding 1 to shift it to 1 and -1
                    parser.Optional(parser.OneOf(out sign, new Dictionary<char, int> { { '+', 0 }, { '-', -2 } }));
                    sign++;
                }

                parser.Required(parser.Integer(out int value));
                value *= sign;
                if (!parser.Complete)
                {
                    Console.WriteLine($"Ignoring excess garbage after number: {line}");
                }
                return value;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid number '{line}': {e.Message}");
                return null;
            }
        }
    }
}
