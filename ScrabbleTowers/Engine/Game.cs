using System;
using System.Collections.Generic;
using System.Linq;

namespace ScrabbleTowers.Engine
{
    public class Game
    {
        private class InternalLetter
        {
            public InternalLetter(Letter letter, int offset)
            {
                Letter = letter;
                Offset = offset;
            }

            public Letter Letter { get; }
            public int Offset { get; }
        }

        private const int MAX_BITS = 64;
        private const int MAX_WORD_LENGTH = 9;

        private readonly string Language;
        private readonly IDictionary<char, InternalLetter> LetterMap;
        private readonly ulong WildcardBit;

        public Game(string language, ICollection<Letter> letters)
        {
            Language = language;
            LetterMap = ToLetterMap(letters, out int wildcardOffset);
            WildcardBit = 1UL << wildcardOffset;
        }

        public bool HasWildcard(ulong bitword)
        {
            return (bitword & WildcardBit) == WildcardBit;
        }

        public ulong GetBitword(string word)
        {
            if (word.Length > MAX_WORD_LENGTH)
                return 0UL;

            var bitword = 0UL;
            var hasWildcard = false;

            foreach (var character in word)
            {
                var charAdded = false;

                if (LetterMap.TryGetValue(character, out var letter))
                {
                    var offset = letter.Offset;

                    for (var i = 0; i < letter.Letter.Count; i++)
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
                    if (!hasWildcard)
                    {
                        bitword |= WildcardBit;
                        hasWildcard = true;
                    }
                    else
                    {
                        return 0UL;
                    }
                }
            }

            return bitword;
        }

        private IDictionary<char, InternalLetter> ToLetterMap(IEnumerable<Letter> letters, out int wildcardOffset)
        {
            var offsets = new Dictionary<char, InternalLetter>();
            int position = 0;

            foreach (var letter in letters.OrderBy(l => l.Glyph))
            {
                offsets.Add(letter.Glyph, new InternalLetter(letter, position));
                position += letter.Count;
            }

            wildcardOffset = position;

            if (position >= MAX_BITS)
                throw new InvalidOperationException("Number of bits in bit word exhausted");

            return offsets;
        }
    }
}
