using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day2
{
    public class B
    {
        public async Task Run()
        {
            string[] lines = await File.ReadAllLinesAsync(Path.Combine("Day2", "input.txt"));
            int count = lines.Count(ValidatePassword);
            Console.WriteLine($"Invalid passwords: {count}, Total passwords: {lines.Length}");
        }

        private bool ValidatePassword(string line)
        {
            try
            {
                var parser = new Parser(line);
                parser.Required(parser.Integer(out int posA));
                parser.Required(parser.Char('-'));
                parser.Required(parser.Integer(out int posB));
                parser.Optional(parser.Whitespace());
                parser.Required(parser.Letter(out char letter));
                parser.Required(parser.Char(':'));
                parser.Optional(parser.Whitespace());
                string password = parser.Remainder.Trim();

                Console.WriteLine($"PosA: {posA}, PosB: {posB}, Letter: {letter}, Password: {password}");

                return
                    (
                        1 <= posA && posA <= password.Length
                        && password[posA - 1] == letter
                    ) != (
                        1 <= posB && posB <= password.Length
                        && password[posB - 1] == letter
                    );
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
