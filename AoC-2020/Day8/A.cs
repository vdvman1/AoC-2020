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

        public static async Task Run()
        {
            (Instruction inst, int offset)[] instructions = await ReadInstructions();

            (bool halted, int accumulator) = RunUntilHaltOrLoop(instructions);
            if(halted)
            {
                Console.WriteLine($"No loop detected, accumulator: {accumulator}");
            }
            else
            {
                Console.WriteLine($"Value of accumulator before loop detected: {accumulator}");
            }
        }

        public static async Task<(Instruction inst, int offset)[]> ReadInstructions()
        {
            string[] lines = await Loading.Load(nameof(Day8));
            return lines.Select(ParseInstruction).ToArray();
        }

        private static (Instruction inst, int offset) ParseInstruction(string line)
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

        public static (bool halted, int accumulator) RunUntilHaltOrLoop(IList<(Instruction inst, int offset)> instructions)
        {
            int accumulator = 0;
            int index = 0;
            var visited = new HashSet<int>();

            while (visited.Add(index))
            {
                if (index >= instructions.Count)
                {
                    return (halted: true, accumulator);
                }

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

            return (halted: false, accumulator);
        }
    }
}
