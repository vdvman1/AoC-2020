using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AoC_2020.Day8.A;

namespace AoC_2020.Day8
{
    public class B
    {
        public static async Task Run()
        {
            (Instruction inst, int offset)[] instructions = await ReadInstructions();

            var repairs =
                Enumerable.Range(0, instructions.Length).AsParallel().AsUnordered().Select(i =>
                {
                    Instruction newInst;
                    switch (instructions[i].inst)
                    {
                        case Instruction.jmp:
                            newInst = Instruction.nop;
                            break;
                        case Instruction.nop:
                            newInst = Instruction.jmp;
                            break;
                        default:
                            return (i, halted: false, accumulator: 0);
                    }

                    var repairedInstructions = new DiffList<(Instruction inst, int offset)>(instructions)
                    {
                        [i] = (newInst, instructions[i].offset)
                    };

                    (bool halted, int accumulator) = RunUntilHaltOrLoop(repairedInstructions);
                    return (i, halted, accumulator);
                })
                .Where(status => status.halted);

            foreach ((int i, _, int accumulator) in repairs)
            {
                Console.WriteLine($"Repaired instruction {i}, accumulator: {accumulator}");
            }
        }
    }
}
