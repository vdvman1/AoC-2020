using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public class Parser
    {
        public delegate Errors? ParserFunc<T>(out T value);

        // TODO: Include position when reporting errors

        private readonly string Str;
        private int Pos = 0;
        private readonly List<Errors> PastOptionalErrors = new List<Errors>();

        public Parser(string str)
        {
            Str = str;
        }

        public string Remainder => Str[Pos..];
        public bool Complete => Pos == Str.Length;

        public void Required(Errors? errors)
        {
            if(errors is null)
            {
                PastOptionalErrors.Clear();
                return;
            }

            try
            {
                errors.ValidateOrThrow(PastOptionalErrors);
                PastOptionalErrors.Clear();
                PastOptionalErrors.Add(errors);
            }
            catch
            {
                PastOptionalErrors.Clear();
                throw;
            }
        }

        public void Optional(Errors? errors)
        {
            if (errors?.AnyErrors == true) PastOptionalErrors.Add(errors);
        }

        public static Errors? Repeat<T>(int count, List<T> values, ParserFunc<T> parser)
        {
            // TODO: Include required number of repetitions in error messages

            Errors? prevErrors = null;
            for (int i = 0; i < count; i++)
            {
                if(parser(out T value) is Errors errors)
                {
                    if(errors.Optional || !errors.AnyErrors)
                    {
                        prevErrors = errors;
                        values.Add(value);
                    }
                    else
                    {
                        errors.Merge(prevErrors);
                        return errors;
                    }
                }
                else
                {
                    prevErrors = null;
                    values.Add(value);
                }
            }

            return prevErrors;
        }

        public bool PeekChar(out char c)
        {
            if (Pos < Str.Length)
            {
                c = Str[Pos];
                return true;
            }

            c = default;
            return false;
        }

        public Errors? AnyChar(out char c)
        {
            if(PeekChar(out c))
            {
                Pos++;
                return null;
            }

            c = default;
            return new Errors("any character");
        }

        public Errors? Char(char c)
        {
            if(PeekChar(out char ch) && ch == c)
            {
                Pos++;
                return null;
            }

            return new Errors(c.ToString());
        }

        public Errors? Digit(out int digit)
        {
            if(
                PeekChar(out char c)
                && c is >= '0' and <= '9'
            )
            {
                Pos++;
                digit = c - '0';
                return null;
            }

            digit = default;
            return new Errors("decimal digit");
        }

        public Errors? Integer(out int value)
        {
            Errors? errors = Digit(out value);
            if (errors?.AnyErrors == true)
            {
                return errors;
            }

            // No leading 0s
            if (value == 0) return null;

            errors = Digit(out int digit);
            while(errors is null || !errors.AnyErrors)
            {
                value = value * 10 + digit;
                errors = Digit(out digit);
            }

            return errors with { Optional = true };
        }

        public Errors Whitespace()
        {
            if(!PeekChar(out char c) || !char.IsWhiteSpace(c))
            {
                return new Errors("whitespace");
            }
            Pos++;

            while(PeekChar(out c) && char.IsWhiteSpace(c))
            {
                Pos++;
            }

            return new Errors("whitespace") { Optional = true };
        }

        public Errors? Letter(out char letter)
        {
            if (
                PeekChar(out letter)
                && char.IsLetter(letter)
            )
            {
                Pos++;
                return null;
            }

            letter = default;
            return new Errors("letter");
        }

        public record Errors(List<string> ExpectedChars, List<string> Other, bool Optional)
        {
            public bool AnyErrors => ExpectedChars.Count != 0 || Other.Count != 0;

            public Errors(params string[] expected) : this(expected.ToList(), new(), false) { }

            public void Merge(Errors? errors)
            {
                if (errors is null) return;

                ExpectedChars.AddRange(errors.ExpectedChars);
                Other.AddRange(errors.Other);
            }

            public void ValidateOrThrow(params Errors?[] prevErrors) => ValidateOrThrow(prevErrors.Where(e => e is not null)!);

            public void ValidateOrThrow(IEnumerable<Errors> prevErrors)
            {
                if (Optional || !AnyErrors) return;

                var expectedChars = ExpectedChars.Concat(prevErrors.SelectMany(e => e.ExpectedChars)).ToList();
                var other = Other.Concat(prevErrors.SelectMany(e => e.Other)).ToList();

                if (expectedChars.Count == 0)
                {
                    if(other.Count == 0)
                    {
                        return;
                    }

                    throw new ParseException(other.JoinWithLast(", ", ", or "));
                }

                var msg = expectedChars.JoinWithLast(", ", ", or ");
                if(other.Count == 0)
                {
                    throw new ParseException(msg);
                }

                throw new ParseException($"{other.JoinWithLast(", ", ", or ")}, also expected {msg}");
            }
        }
    }

    public class ParseException : Exception
    {
        public ParseException(string msg) : base(msg) { }
    }
}
