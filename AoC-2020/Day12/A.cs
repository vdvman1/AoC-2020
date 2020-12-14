using AoC_2020.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Day12
{
    public class A
    {
        // Discriminated union pattern
        public record Action
        {
            protected Action() { }

            public record Rotate(int Degrees) : Action { }
            public record Forward(int Amount) : Action { }
            public record Move(int Dx, int Dy) : Action
            {
                public static Move FromAngle(int degrees, int value)
                {
                    switch(degrees % 360)
                    {
                        case 0:
                            return new Move(0, value);
                        case 90:
                            return new Move(value, 0);
                        case 180:
                            return new Move(0, -value);
                        case 270:
                            return new Move(-value, 0);
                        default:
                            Console.WriteLine($"Angles must be multiples of 90, got: {degrees}");
                            throw new InvalidOperationException($"Angles must be multiples of 90, got: {degrees}");
                    }
                }

                public void Apply(ref int x, ref int y)
                {
                    x += Dx;
                    y += Dy;
                }
            }
        }

        public enum ActionType
        {
            // NOTE: Must start with north and the cardinal directions must be in clockwise order
            North, East, South, West, Foward, TurnLeft, TurnRight
        }

        private static readonly IReadOnlyDictionary<char, ActionType> ActionTypeParser = new Dictionary<char, ActionType>
        {
            { 'N', ActionType.North },
            { 'E', ActionType.East },
            { 'S', ActionType.South },
            { 'W', ActionType.West },
            { 'F', ActionType.Foward },
            { 'L', ActionType.TurnLeft },
            { 'R', ActionType.TurnRight }
        };

        public static async Task<IEnumerable<Action>> Load()
        {
            string[] lines = await Loading.Load(nameof(Day12));
            return lines.Select(ParseAction).WhereNotNull();
        }

        private static Action? ParseAction(string line)
        {
            try
            {
                var parser = new Parser(line);
                parser.Required(parser.OneOf(out ActionType type, ActionTypeParser));
                parser.Required(parser.Integer(out int value));
                switch (type)
                {
                    case >= ActionType.North and <= ActionType.West:
                        return Action.Move.FromAngle((int)type * 90, value);
                    case ActionType.Foward:
                        return new Action.Forward(value);
                    case ActionType.TurnLeft:
                        value = -value;
                        goto case ActionType.TurnRight;
                    case ActionType.TurnRight:
                        if (value % 90 != 0)
                        {
                            Console.WriteLine($"Rotations must be multiples of 90, got: {value}");
                            return null;
                        }
                        return new Action.Rotate(value % 360);
                    default:
                        Console.WriteLine($"Unknown action type: {type}");
                        return null;
                }
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Invalid action '{line}': {e.Message}");
                return null;
            }
        }

        public static async Task Run()
        {
            int x = 0;
            int y = 0;
            int angle = (int)ActionType.East * 90;

            foreach (Action action in await Load())
            {
                switch (action)
                {
                    case Action.Move move:
                        move.Apply(ref x, ref y);
                        break;
                    case Action.Rotate(int degrees):
                        angle = (angle + degrees) % 360;
                        if(angle < 0)
                        {
                            angle += 360;
                        }
                        break;
                    case Action.Forward(int amount):
                        Action.Move.FromAngle(angle, amount).Apply(ref x, ref y);
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
