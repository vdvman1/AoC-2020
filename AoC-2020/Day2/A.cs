using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day2
{
    public class A
    {
        public static async Task Run()
        {
            string[] lines = await Loading.Load(nameof(Day2));
            int count = lines.Count(ValidatePassword);
            Console.WriteLine($"Invalid passwords: {count}, Total passwords: {lines.Length}");
        }

        private static bool ValidatePassword(string line)
        {
            try
            {
                var parser = new Parser(line);
                parser.Required(parser.Integer(out int min));
                parser.Required(parser.Char('-'));
                parser.Required(parser.Integer(out int max));
                parser.Optional(parser.Whitespace());
                parser.Required(parser.Letter(out char letter));
                parser.Required(parser.Char(':'));
                parser.Optional(parser.Whitespace());
                string password = parser.Remainder.Trim();

                Console.WriteLine($"Min: {min}, Max: {max}, Letter: {letter}, Password: {password}");

                int count = password.Count(c => c == letter);

                return min <= count && count <= max;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Failed to parse '{line}': {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
