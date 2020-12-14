using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day12
{
    public class B
    {
        public static async Task Run()
        {
            int x = 0;
            int y = 0;
            int waypointX = 10;
            int wayPointY = 1;

            foreach (A.Action action in await A.Load())
            {
                switch(action)
                {
                    case A.Action.Move move:
                        move.Apply(ref waypointX, ref wayPointY);
                        break;
                    case A.Action.Forward(int amount):
                        x += waypointX * amount;
                        y += wayPointY * amount;
                        break;
                    case A.Action.Rotate(int degrees):
                        degrees %= 360;
                        if(degrees < 0)
                        {
                            degrees += 360;
                        }

                        // Rotations based on the common rotation matrices for multiples of 90 degrees
                        // Note most references for these matrices use counter-clockwise rotations, we are using clockwise
                        // Since the waypoint is always considered centered around the boat we can just rotate the waypoint coord and don't need to worry about any translations
                        switch(degrees)
                        {
                            case 0:
                                break;
                            case 90:
                                (waypointX, wayPointY) = (wayPointY, -waypointX);
                                break;
                            case 180:
                                (waypointX, wayPointY) = (-waypointX, -wayPointY);
                                break;
                            case 270:
                                (waypointX, wayPointY) = (-wayPointY, waypointX);
                                break;
                            default:
                                Console.WriteLine($"Rotations must be multiples of 90, got: {degrees}");
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown action: {action.GetType().Name}");
                        break;
                }
            }

            Console.WriteLine($"Manhattan distance: {Math.Abs(x) + Math.Abs(y)}");
        }
    }
}
