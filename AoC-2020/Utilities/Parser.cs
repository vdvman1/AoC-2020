﻿using System;
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

        public void Required()
        {
            if (PastOptionalErrors.Count == 0) return;

            var errors = PastOptionalErrors[^1];
            PastOptionalErrors.RemoveAt(PastOptionalErrors.Count - 1);
            Required(errors);
        }

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

        public bool Optional(Errors? errors)
        {
            if (errors?.AnyErrors == true)
            {
                PastOptionalErrors.Add(errors);
                return false;
            }

            return true;
        }

        public static Errors? Repeat<T>(int count, ParserFunc<T> parser, Action<T> sink)
        {
            // TODO: Include required number of repetitions in error messages

            Errors? prevErrors = null;
            for (int i = 0; i < count; i++)
            {
                if (parser(out T value) is Errors errors)
                {
                    if (errors.Optional || !errors.AnyErrors)
                    {
                        prevErrors = errors;
                        sink(value);
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
                    sink(value);
                }
            }

            return prevErrors;
        }

        public static Errors? Repeat<T>(int count, List<T> values, ParserFunc<T> parser) => Repeat(count, parser, values.Add);

        public static Errors? Repeat<T>(int count, ParserFunc<T> parser) => Repeat(count, parser, v => { });

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

        public void Skip()
        {
            if(Pos < Str.Length)
            {
                Pos++;
            }
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

        public Errors? HexDigit(out int digit)
        {
            if (PeekChar(out char c))
            {
                switch(c)
                {
                    case >= '0' and <= '9':
                        Pos++;
                        digit = c - '0';
                        return null;
                    case >= 'a' and <= 'f':
                        Pos++;
                        digit = c - 'a' + 10;
                        return null;
                }
            }

            digit = default;
            return new Errors("hexadecimal digit");
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

        public Errors? Match(string str)
        {
            if (
                Pos + str.Length > Str.Length
                || !MemoryExtensions.Equals(Str.AsSpan(Pos, str.Length), str, StringComparison.Ordinal)
            )
            {
                return new Errors(str);
            }

            Pos += str.Length;
            return null;
        }

        public Errors? OneOf<T>(out T? matched, Trie<T> trie)
        {
            if(!PeekChar(out char c))
            {
                if(trie.HasValue)
                {
                    matched = trie.Value;
                    return null;
                }

                matched = default;
                return new Errors(trie.Prefixes.Select(char.ToString).ToArray());
            }

            var chars = new Dictionary<char, TrieNode<T>>(trie.Children);

            do
            {
                if (!chars.TryGetValue(c, out TrieNode<T>? node))
                {
                    matched = default;
                    return new Errors(chars.Keys.Select(char.ToString).ToArray());
                }

                if(Match(node.Key) is Errors errors && errors.AnyErrors)
                {
                    matched = default;
                    return errors;
                }

                // Shortest match first
                if(node.HasValue)
                {
                    matched = node.Value;
                    return null;
                }

                chars.Clear();
                foreach (var child in node.Children)
                {
                    chars.Add(child.Key, child.Value);
                }

                if(chars.Count == 0)
                {
                    matched = default;
                    return null; // TODO: warning on matching a case with no value
                }
            } while (PeekChar(out c));

            matched = default;
            return new Errors(chars.Keys.Select(char.ToString).ToArray());
        }

        public Errors? OneOf(out int index, params char[] chars) => OneOf(out index, chars.Select((c, i) => (c, i)).ToDictionary(c => c.c, c => c.i));

        public Errors? OneOf<T>(out T? id, IReadOnlyDictionary<char, T> chars)
        {
            if(PeekChar(out char c) && chars.TryGetValue(c, out id))
            {
                Pos++;
                return null;
            }

            id = default;
            return new Errors(chars.Keys.Select(char.ToString).ToArray());
        }
        public Errors? AnyUntil(out string chars, string match, bool consumeMatch = true)
        {
            if(Pos == Str.Length)
            {
                chars = string.Empty;
                return new Errors(match);
            }

            int index = Str.IndexOf(match, Pos);
            if (index < 0)
            {
                chars = string.Empty;
                return new Errors(match);
            }

            chars = Str[Pos..index];
            Pos = consumeMatch ? index + match.Length : index;
            return null;
        }

        public string AnyUntilOptional(string match, bool consumeMatch = true)
        {
            if (Pos == Str.Length)
            {
                return string.Empty;
            }

            int index = Str.IndexOf(match, Pos);
            if (index < 0)
            {
                return Remainder;
            }

            var chars = Str[Pos..index];
            Pos = consumeMatch ? index + match.Length : index;
            return chars;
        }

        public string AnyUntilOneOfOptional(out int? index, params char[] chars) => AnyUntilOneOfOptional(out index, chars.Select((c, i) => (c, i)).ToDictionary(c => c.c, c => c.i));

        public string AnyUntilOneOfOptional(out int? id, IReadOnlyDictionary<char, int> chars)
        {
            if(Pos == Str.Length)
            {
                id = null;
                return string.Empty;
            }

            var builder = new StringBuilder();
            while(PeekChar(out char c))
            {
                Pos++;
                if(chars.TryGetValue(c, out int foundId))
                {
                    id = foundId;
                    return builder.ToString();
                }
                builder.Append(c);
            }

            id = null;
            return builder.ToString();
        }

        public Errors? AnyUntilOneOf(out string value, out int index, params char[] chars) => AnyUntilOneOf(out value, out index, chars.Select((c, i) => (c, i)).ToDictionary(c => c.c, c => c.i));

        public Errors? AnyUntilOneOf(out string value, out int id, IReadOnlyDictionary<char, int> chars)
        {
            if (Pos == Str.Length)
            {
                id = default;
                value = string.Empty;
                return new Errors(chars.Keys.Select(char.ToString).ToArray());
            }

            var builder = new StringBuilder();
            while (PeekChar(out char c))
            {
                Pos++;
                if (chars.TryGetValue(c, out int foundId))
                {
                    id = foundId;
                    value = builder.ToString();
                    return null;
                }
                builder.Append(c);
            }

            id = default;
            value = builder.ToString();
            return new Errors(chars.Keys.Select(char.ToString).ToArray());
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
                    throw new ParseException($"Expected {msg}");
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
