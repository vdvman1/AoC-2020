using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day4
{
    public class A
    {
        private static readonly IReadOnlySet<string> RequiredKeys = new HashSet<string> { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
        private static readonly IReadOnlySet<string> ValidKeys = new HashSet<string>(RequiredKeys) { "cid" };

        public static async Task Run()
        {
            string[] lines = await Loading.Load(nameof(Day4));
            int validCount = 0;
            for (int l = 0; l < lines.Length; l++)
            {
                var keys = new HashSet<string>();
                bool valid = true;
                while(l < lines.Length && !string.IsNullOrWhiteSpace(lines[l]))
                {
                    foreach (var pair in lines[l].Split())
                    {
                        var parts = pair.Split(':');
                        if(parts.Length != 2)
                        {
                            Console.WriteLine($"Passport key:value pair with an unexpected number of ':' characters: {pair}");
                            valid = false;
                        }
                        else if(!ValidKeys.Contains(parts[0]))
                        {
                            Console.WriteLine($"Invalid key: {parts[0]}");
                            valid = false;
                        }
                        else if(!keys.Add(parts[0]))
                        {
                            Console.WriteLine($"Duplicate key: {parts[0]}");
                            valid = false;
                        }
                    }

                    l++;
                }

                if(valid)
                {
                    var missing = RequiredKeys.Where(k => !keys.Contains(k)).ToList();
                    if(missing.Count != 0)
                    {
                        Console.WriteLine($"Missing keys: {missing.JoinWithLast(", ", ", and ")}");
                    }
                    else
                    {
                        validCount++;
                    }
                }
            }

            Console.WriteLine($"Valid passports: {validCount}");
        }
    }
}
