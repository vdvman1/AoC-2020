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

        public static async Task<IEnumerable<int>> LoadNumbers(string name)
        {
            string[] lines = await Load(name);
            return ((IEnumerable<string>)lines).Select(ParseNumber).WhereNotNull();
        }

        public static int? ParseNumber(string line)
        {
            try
            {
                var parser = new Parser(line);
                parser.Required(parser.Integer(out int value));
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
