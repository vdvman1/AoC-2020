using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day11
{
    public class A
    {
        public enum SeatState
        {
            Empty,
            Occupied,
            Floor
        }

        private static readonly IReadOnlyDictionary<char, SeatState> stateParser = new Dictionary<char, SeatState>
        {
            { 'L', SeatState.Empty },
            { '#', SeatState.Occupied },
            { '.', SeatState.Floor }
        };

        public static readonly IReadOnlyList<(int x, int y)> Offsets = Array.AsReadOnly(new[]
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1)
        });

        public static async Task<SeatState[,]?> Load()
        {
            string[] lines = await Loading.Load(nameof(Day11));
            List<List<SeatState>> rows = lines.Where(l => l.Length > 0).Select(ParseRow).WhereNotNull().ToList();
            if (rows.Count == 0)
            {
                Console.WriteLine("No valid rows parsed");
                return null;
            }

            var width = rows[0].Count;
            var seats = new SeatState[rows.Count, width];

            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].Count != width)
                {
                    Console.WriteLine("Rows have varying length");
                    return null;
                }

                for (int j = 0; j < width; j++)
                {
                    seats[i, j] = rows[i][j];
                }
            }

            return seats;
        }

        private static List<SeatState>? ParseRow(string line)
        {
            try
            {
                var parser = new Parser(line);
                var seats = new List<SeatState>();
                parser.Required(Parser.Repeat(line.Length, seats, (out SeatState state) => parser.OneOf(out state, stateParser)));
                return seats;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid seat row '{line}': {e.Message}");
                return null;
            }
        }

        public static async Task Run()
        {
            if(await Load() is not SeatState[,] seats)
            {
                return;
            }

            int height = seats.GetLength(0);
            int width = seats.GetLength(1);
            var updatedSeats = new SeatState[height, width];

            bool changed;
            do
            {
                changed = false;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        updatedSeats[i, j] = 
                            (
                                Offsets.Count(off => IsOccupied(seats, j + off.x, i + off.y)),
                                seats[i, j]
                            )
                            switch
                            {
                                (0   , SeatState.Empty)    => SeatState.Occupied,
                                (>= 4, SeatState.Occupied) => SeatState.Empty,
                                (_   , SeatState state)    => state
                            };

                        if(updatedSeats[i, j] != seats[i, j])
                        {
                            changed = true;
                        }
                    }
                }

                (seats, updatedSeats) = (updatedSeats, seats);
            } while (changed);

            Console.WriteLine($"Number of occupied seats: {seats.Flatten().Count(s => s == SeatState.Occupied)}");
        }

        public static bool InRange(SeatState[,] seats, int x, int y) =>
            0 <= y && y < seats.GetLength(0)
            && 0 <= x && x < seats.GetLength(1);

        private static bool IsOccupied(SeatState[,] seats, int x, int y) => InRange(seats, x, y) && seats[y, x] == SeatState.Occupied;
    }
}
