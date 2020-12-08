using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020.Utilities
{
    public class Trie<T> : IEnumerable<KeyValuePair<string, T>>
    {
        private bool hasValue = false;

        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue
        {
            get => hasValue;
            set
            {
                if(!value)
                {
                    Value = default;
                }
                hasValue = value;
            }
        }

        public T? Value;

        private readonly Dictionary<char, TrieNode<T>> Nodes = new();

        public IEnumerable<KeyValuePair<char, TrieNode<T>>> Children => Nodes.Select(c => c); // Ensure caller can't cast back to dictionary
        public IEnumerable<char> Prefixes => Nodes.Keys;

        public void Add(ReadOnlySpan<char> key, T value)
        {
            if(key.Length == 0)
            {
                HasValue = true;
                Value = value;
                return;
            }

            if(!Nodes.TryGetValue(key[0], out TrieNode<T>? child))
            {
                child = new TrieNode<T>(key);
                Nodes.Add(key[0], child);
            }

            child.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => Nodes.Values.SelectMany(c => c).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class TrieNode<T> : IEnumerable<KeyValuePair<string, T>>
    {
        public string Key { get; private set; }

        private bool hasValue = false;

        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue
        {
            get => hasValue;
            set
            {
                if (!value)
                {
                    Value = default;
                }
                hasValue = value;
            }
        }

        public T? Value;

        private Dictionary<char, TrieNode<T>> Nodes = new();

        internal TrieNode(ReadOnlySpan<char> key) => Key = new string(key);

        private TrieNode(string key, Dictionary<char, TrieNode<T>> children, bool hasValue, T? value)
        {
            Key = key;
            Nodes = children;
            HasValue = hasValue;
            Trace.Assert(!HasValue || value is not null);
            Value = value;
        }

        public IEnumerable<KeyValuePair<char, TrieNode<T>>> Children => Nodes.Select(c => c); // Ensure caller can't cast back to dictionary

        public void Add(ReadOnlySpan<char> key, T value)
        {
            if (MemoryExtensions.Equals(Key, key, StringComparison.Ordinal))
            {
                Value = value;
                HasValue = true;
                return;
            }

            if (key.StartsWith(Key))
            {
                key = key[Key.Length..];
                if (!Nodes.TryGetValue(key[0], out TrieNode<T>? node))
                {
                    node = new TrieNode<T>(key);
                    Nodes.Add(key[0], node);
                }

                node.Add(key, value);
            }
            else
            {
                int i = 0;
                while (i < key.Length && i < Key.Length && key[i] == Key[i])
                {
                    i++;
                }

                var node = new TrieNode<T>(Key[i..], Nodes, HasValue, value);
                Nodes = new Dictionary<char, TrieNode<T>>
                {
                    { Key[i], node }
                };

                Key = new string(key[..i]);
                key = key[i..];
                if (key.IsEmpty)
                {
                    Value = value;
                    HasValue = true;
                }
                else
                {
                    Value = default;
                    HasValue = false;
                    node = new TrieNode<T>(key)
                    {
                        { key, value }
                    };
                    Nodes.Add(key[0], node);
                }
            }

        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            if(HasValue)
            {
                yield return new KeyValuePair<string, T>(Key, Value);
            }

            foreach (KeyValuePair<string, T> pair in Nodes.Values.SelectMany(c => c))
            {
                yield return new KeyValuePair<string, T>(Key + pair.Key, pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
