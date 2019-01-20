using System;
using System.Collections.Generic;
using System.Linq;
using ScrabbleTowers.Utils;

namespace ScrabbleTowers.Engine
{
    public class Game
    {
        private class LetterGroup
        {
            public LetterGroup(IEnumerable<Letter> letters, int offset)
            {
                Letters = letters.ToList();
                Offset = offset;
            }

            public ICollection<Letter> Letters { get; }
            public int Offset { get; }
        }

        private const int MAX_BITS = 64;
        private const int MAX_WORD_LENGTH = 9;

        private readonly string Language;
        private readonly IEnumerable<Letter> Letters;
        private readonly IDictionary<char, LetterGroup> LetterMap;
        private readonly ulong WildcardBit;
        private readonly Random Rng = new Random();

        public Game(string language, IEnumerable<Letter> letters)
        {
            Language = language;
            Letters = letters;
            LetterMap = ToLetterMap(letters, out int wildcardOffset);
            WildcardBit = 1UL << wildcardOffset;
        }

        public Board GetRandomBoard()
        {
            var shuffled = Letters
                .OrderBy(l => Rng.Next())
                .GetEnumerator()
                .ToEnumerable();

            var board = new Board();
            board.AddStack(0, shuffled.Take(5));
            board.AddStack(1, shuffled.Take(5));
            board.AddStack(2, shuffled.Take(5));
            board.AddStack(3, shuffled.Take(5));
            board.AddStack(4, shuffled.Take(4));
            board.AddStack(5, shuffled.Take(5));
            board.AddStack(6, shuffled.Take(5));
            board.AddStack(7, shuffled.Take(5));
            board.AddStack(8, shuffled.Take(5));
            return board;
        }

        public bool HasWildcard(ulong bitword)
        {
            return (bitword & WildcardBit) == WildcardBit;
        }

        public ulong GetBitword(string word, bool wildcard = false)
        {
            if (word.Length > MAX_WORD_LENGTH)
                return 0UL;

            var bitword = wildcard ? WildcardBit : 0UL;

            foreach (var character in word)
            {
                var charAdded = false;

                if (LetterMap.TryGetValue(character, out var group))
                {
                    var offset = group.Offset;

                    for (var i = 0; i < group.Letters.Count; i++)
                    {
                        var bit = 1UL << (offset + i);

                        if ((bitword & bit) == 0UL)
                        {
                            bitword |= bit;
                            charAdded = true;
                            break;
                        }
                    }
                }

                if (!charAdded)
                {
                    if (!wildcard)
                    {
                        bitword |= WildcardBit;
                        wildcard = true;
                    }
                    else
                    {
                        return 0UL;
                    }
                }
            }

            return bitword;
        }

        private IDictionary<char, LetterGroup> ToLetterMap(IEnumerable<Letter> letters, out int wildcardOffset)
        {
            var result = new Dictionary<char, LetterGroup>();
            int offset = 0;

            foreach (var kv in letters.GroupBy(l => l.Glyph).OrderBy(g => g.Key))
            {
                var group = new LetterGroup(kv, offset);
                result.Add(kv.Key, group);
                offset += group.Letters.Count;
            }

            wildcardOffset = offset;

            if (offset >= MAX_BITS)
                throw new InvalidOperationException("Number of bits in bit word exhausted");

            return result;
        }
    }
}
