using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScrabbleTowers.Utils;

namespace ScrabbleTowers.Engine
{
    public class DictionaryStats
    {   
        public int Lines { get; set; }
        public int Words { get; set; }
    }

    public class Dictionary
    {
        private readonly Game Game;
        private List<Word> _words;
        private DictionaryStats _stats = new DictionaryStats();

        public Dictionary(Game game)
        {
            Game = game;
        }

        public int Lines => _stats.Lines;

        public int Words => _stats.Words;

        public IEnumerable<string> MatchWords(Board board)
        {
            var boardLetters = new string(board.Select(t => t.Glyph ?? '*').ToArray());
            var pattern = Game.GetBitword(boardLetters);

            if (pattern == 0UL)
                throw new InvalidOperationException("Board arrangement not possible");

            var isMatch = Game.HasWildcard(pattern)
                ? (Func<Word, ulong, bool>)IsWildcardMatch
                : IsMatch;

            foreach (var word in _words)
            {
                if (isMatch(word, pattern))
                    yield return word.Value;
            }
        }

        public async Task Load(string path)
        {
            _words = new List<Word>();
            _stats = new DictionaryStats();

            using (var file = File.OpenText(path))
            {
                while (!file.EndOfStream)
                {
                    var line = await file.ReadLineAsync();
                    _stats.Lines += 1;

                    if (line != null)
                    {
                        var value = line.ToLowerInvariant().Trim();
                        var bits = Game.GetBitword(value);

                        if (bits != 0UL)
                        {
                            _words.Add(new Word(value, bits));
                            _stats.Words += 1;
                        }
                    }
                }
            }
        }

        private bool IsMatch(Word word, ulong pattern)
        {
            return (word.Bits & pattern) == word.Bits;
        }

        private bool IsWildcardMatch(Word word, ulong pattern)
        {
            if (IsMatch(word, pattern))
                return true;

            if (Game.HasWildcard(word.Bits))
                return false;

            var negPattern = ~pattern;
            var negBitword = negPattern & word.Bits;
            return BitUtils.CountSetBits(negBitword) == 1;
        }
    }
}
