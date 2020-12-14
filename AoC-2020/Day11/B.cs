using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AoC_2020.Day11.A;

namespace AoC_2020.Day11
{
    public class B
    {
        public static async Task Run()
        {
            if (await Load() is not SeatState[,] seats)
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
                                Offsets.Count(off => IsOccupiedAlong(seats, j, i, off.x, off.y)),
                                seats[i, j]
                            )
                            switch
                            {
                                (0   , SeatState.Empty)    => SeatState.Occupied,
                                (>= 5, SeatState.Occupied) => SeatState.Empty,
                                (_   , SeatState state)    => state
                            };

                        if (updatedSeats[i, j] != seats[i, j])
                        {
                            changed = true;
                        }
                    }
                }

                (seats, updatedSeats) = (updatedSeats, seats);
            } while (changed);

            Console.WriteLine($"Number of occupied seats: {seats.Flatten().Count(s => s == SeatState.Occupied)}");
        }

        private static bool IsOccupiedAlong(SeatState[,] seats, int x, int y, int dx, int dy)
        {
            while(InRange(seats, x + dx, y + dy))
            {
                x += dx;
                y += dy;
                switch(seats[y, x])
                {
                    case SeatState.Occupied:
                        return true;
                    case SeatState.Empty:
                        return false;
                }
            }

            return false;
        }
    }
}
