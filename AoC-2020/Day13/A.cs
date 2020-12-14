using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day13
{
    public class A
    {
        public static async Task<(int time, List<int?> busses)?> Load()
        {
            string[] lines = await Loading.Load(nameof(Day13));
            switch(lines.Length)
            {
                case < 2:
                    Console.WriteLine("Not enough lines");
                    return null;
                case > 2:
                    Console.WriteLine("Ignoring excess lines");
                    break;
            }

            int time;
            try
            {
                var parser = new Parser(lines[0]);
                parser.Required(parser.Integer(out time));
                if(!parser.Complete)
                {
                    Console.WriteLine($"Ignoring excess garbage after the timestamp: {lines[0]}");
                }
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid timestamp '{lines[0]}': {e.Message}");
                return null;
            }

            var busses = new List<int?>();
            try
            {
                var parser = new Parser(lines[1]);
                do
                {
                    if (parser.Optional(parser.Char('x')))
                    {
                        busses.Add(null);
                    }
                    else
                    {
                        parser.Required(parser.Integer(out int bus));
                        busses.Add(bus);
                    }
                } while (parser.Optional(parser.Char(',')));

                if(!parser.Complete)
                {
                    parser.Required();
                }
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid bus IDs '{lines[1]}': {e.Message}");
                return null;
            }

            return (time, busses);
        }

        public static async Task Run()
        {
            if(await Load() is not (int time, List<int?> allBusses))
            {
                return;
            }

            List<int> busses = allBusses.WhereNotNull().ToList();
            int departureTime = time;
            int? bus = BusDeparting(departureTime, busses);
            while(bus is null)
            {
                departureTime++;
                bus = BusDeparting(departureTime, busses);
            }

            Console.WriteLine($"Time delay * bus ID = {(departureTime - time) * bus.Value}");
        }

        public static int? BusDeparting(int time, IEnumerable<int> busses)
        {
            foreach (int bus in busses)
            {
                if(time % bus == 0)
                {
                    return bus;
                }
            }

            return null;
        }
    }
}
