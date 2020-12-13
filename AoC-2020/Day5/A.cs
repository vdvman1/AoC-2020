using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day5
{
    public class A
    {
        public static async Task Run()
        {
            string[] lines = await Loading.Load(nameof(Day5));
            int maxID = lines.Select(ToId).Where(id => id.HasValue).Select(id => id!.Value).Max();
            Console.WriteLine($"Max boarding pass ID: {maxID}");
        }

        public static int? ToId(string value)
        {
            try
            {
                var parser = new Parser(value);
                const int backBits = 7;
                const int rightBits = 3;
                int id = 0;
                void AddBit(int bit) => id = (id << 1) | bit;

                parser.Required(Parser.Repeat(backBits, (out int bit) => parser.OneOf(out bit, 'F', 'B'), AddBit));
                parser.Required(Parser.Repeat(rightBits, (out int bit) => parser.OneOf(out bit, 'L', 'R'), AddBit));

                if(!parser.Complete)
                {
                    Console.WriteLine($"Too many characters in boarding pass seat '{value}'");
                    return null;
                }

                return id;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid boarding pass seat '{value}': {e.Message}");
                return null;
            }
        }
    }
}
