﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            await Day13.A.Run();
            timer.Stop();
            Console.WriteLine($"Duration: {timer.ElapsedMilliseconds}ms");
        }
    }
}
