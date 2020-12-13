using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day4
{
    public class B
    {
        private const string BirthYear = "byr";
        private const string IssueYear = "iyr";
        private const string ExpYear = "eyr";
        private const string Height = "hgt";
        private const string HairColour = "hcl";
        private const string EyeColour = "ecl";
        private const string PassID = "pid";
        private const string CountryID = "cid";

        private static readonly IReadOnlySet<string> RequiredKeys = new HashSet<string> { BirthYear, IssueYear, ExpYear, Height, HairColour, EyeColour, PassID };

        public static async Task Run()
        {
            string[] lines = await Loading.Load(nameof(Day4));
            int validCount = 0;

            for (int l = 0; l < lines.Length; l++)
            {
                var keys = new HashSet<string>();
                bool valid = true;
                while (l < lines.Length && !string.IsNullOrWhiteSpace(lines[l]))
                {
                    foreach (var pair in lines[l].Split())
                    {
                        var parts = pair.Split(':');
                        if (parts.Length != 2)
                        {
                            Console.WriteLine($"Passport key:value pair with an unexpected number of ':' characters: {pair}");
                            valid = false;
                        }
                        else if(!keys.Add(parts[0]))
                        {
                            Console.WriteLine($"Duplicate key: {parts[0]}");
                            valid = false;
                        }
                        else
                        {
                            switch(parts[0])
                            {
                                case BirthYear:
                                    valid = ValidateYear(parts[1], "birth", 1920, 2002) && valid;
                                    break;
                                case IssueYear:
                                    valid = ValidateYear(parts[1], "issue", 2010, 2020) && valid;
                                    break;
                                case ExpYear:
                                    valid = ValidateYear(parts[1], "expiration", 2020, 2030) && valid;
                                    break;
                                case Height:
                                    valid = ValidateHeight(parts[1]) && valid;
                                    break;
                                case HairColour:
                                    valid = ValidateHairColour(parts[1]) && valid;
                                    break;
                                case EyeColour:
                                    if(parts[1] is not ("amb" or "blu" or "brn" or "grn" or "gry" or "hzl" or "oth"))
                                    {
                                        valid = false;
                                        Console.WriteLine($"Invalid eye colour: {parts[1]}");
                                    }
                                    break;
                                case PassID:
                                    valid = ValidatePassID(parts[1]) && valid;
                                    break;
                                case CountryID:
                                    // Ignore
                                    break;
                                default:
                                    Console.WriteLine($"Invalid key: {parts[0]}");
                                    valid = false;
                                    break;
                            }
                        }
                    }

                    l++;
                }

                var missing = RequiredKeys.Where(k => !keys.Contains(k)).ToList();
                if (missing.Count != 0)
                {
                    Console.WriteLine($"Missing keys: {missing.JoinWithLast(", ", ", and ")}");
                }
                else if (valid)
                {
                    validCount++;
                }
            }

            Console.WriteLine($"Valid passports: {validCount}");
        }

        private static bool ValidateYear(string value, string name, int min, int max)
        {
            try
            {
                var parser = new Parser(value);
                int year = 0;
                parser.Required(Parser.Repeat(4, parser.Digit, (int digit) => year = year * 10 + digit));

                if (!parser.Complete)
                {
                    Console.WriteLine($"Too many characters in {name} year: {value}");
                    return false;
                }

                if (year < min)
                {
                    Console.WriteLine($"{name} year too early: {year}");
                    return false;
                }

                if (year > max)
                {
                    Console.WriteLine($"{name} year too late: {year}");
                    return false;
                }

                return true;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid {name} year '{value}': {e.Message}");
                return false;
            }
        }

        private static bool ValidateHeight(string value)
        {
            try
            {
                var parser = new Parser(value);
                parser.Required(parser.Integer(out int height));
                if (parser.PeekChar(out char c))
                {
                    switch (c)
                    {
                        case 'c':
                            parser.Skip();
                            parser.Required(parser.Char('m'));

                            if (!parser.Complete)
                            {
                                Console.WriteLine($"Too many characters in height: {value}");
                                return false;
                            }

                            if (height < 150)
                            {
                                Console.WriteLine($"Too short: {value}");
                                return false;
                            }

                            if (height > 193)
                            {
                                Console.WriteLine($"Too tall: {value}");
                                return false;
                            }

                            return true;
                        case 'i':
                            parser.Skip();
                            parser.Required(parser.Char('n'));

                            if (!parser.Complete)
                            {
                                Console.WriteLine($"Too many characters in height: {value}");
                                return false;
                            }

                            if (height < 59)
                            {
                                Console.WriteLine($"Too short: {value}");
                                return false;
                            }

                            if (height > 76)
                            {
                                Console.WriteLine($"Too tall: {value}");
                                return false;
                            }

                            return true;
                    }
                }

                Console.WriteLine($"Expected 'cm' or 'in' unit for height: {value}");
                return false;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid height '{value}': {e.Message}");
                return false;
            }
        }

        private static bool ValidateHairColour(string value)
        {
            try
            {
                var parser = new Parser(value);
                parser.Required(parser.Char('#'));
                parser.Required(Parser.Repeat<int>(6, parser.HexDigit));
                // No validation on actual hex value needed

                if(!parser.Complete)
                {
                    Console.WriteLine($"Too many characters in hair colour: {value}");
                    return false;
                }
                return true;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid hair colour '{value}': {e.Message}");
                return false;
            }
        }

        private static bool ValidatePassID(string value)
        {
            try
            {
                var parser = new Parser(value);
                parser.Required(Parser.Repeat<int>(9, parser.Digit));
                // No validation on actual passport ID needed

                if(!parser.Complete)
                {
                    Console.WriteLine($"Too many characters in passport ID: {value}");
                    return false;
                }
                return true;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid passport ID '{value}': {e.Message}");
                return false;
            }
        }
    }
}
