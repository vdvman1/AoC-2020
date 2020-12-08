using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day8
{
    public class A
    {
        public enum Instruction
        {
            Unknown, acc, jmp, nop
        }

        private static readonly Trie<Instruction> InstructionMatcher = new()
        {
            { nameof(Instruction.acc), Instruction.acc },
            { nameof(Instruction.jmp), Instruction.jmp },
            { nameof(Instruction.nop), Instruction.nop }
        };

        public async Task Run()
        {
            string[] lines = await File.ReadAllLinesAsync(Path.Combine("Day8", "input.txt"));
            (Instruction inst, int offset)[] instructions = lines.Select(ParseInstruction).ToArray();
            
            int accumulator = 0;
            int index = 0;
            var visited = new HashSet<int>();

            while (visited.Add(index))
            {
                switch (instructions[index].inst)
                {
                    case Instruction.jmp:
                        index += instructions[index].offset;
                        break;
                    case Instruction.acc:
                        accumulator += instructions[index].offset;
                        index++;
                        break;
                    case Instruction.Unknown:
                    case Instruction.nop:
                    default:
                        index++;
                        break;
                }
            }

            Console.WriteLine($"Value of accumulator before loop detected: {accumulator}");
        }

        public static (Instruction inst, int offset) ParseInstruction(string line)
        {
            try
            {
                var parser = new Parser(line);
                parser.Required(parser.OneOf(out Instruction inst, InstructionMatcher));
                if(inst == Instruction.Unknown)
                {
                    Console.WriteLine($"Unknown instruction '{line}'");
                    return (Instruction.Unknown, 0);
                }

                parser.Optional(parser.Whitespace());
                parser.Required(parser.OneOf(out int sign, '+', '-'));
                parser.Required(parser.Integer(out int value));

                return (inst, sign == 0 ? value : -value);
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid instruction '{line}': {e.Message}");
                return (Instruction.Unknown, 0);
            }
        }
    }
}
