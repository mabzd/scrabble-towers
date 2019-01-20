using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrabbleTowers.Engine;
using ScrabbleTowers.Utils;

namespace ScrabbleTowers
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            var dict = await LoadDictionary();
            //UserInput(dict);
            BoardHistogram(dict);
        }

        private static void BoardTest(Dictionary dict)
        {
            while (true)
            {
                var board = Games.Polish.GetRandomBoard();

                Print(board);

                var layer = board.GetTopLayer();
                var longestWord = dict
                    .MatchWords(layer)
                    .Where(layer.IsAllowed)
                    .OrderBy(w => w.Length)
                    .LastOrDefault();

                Console.WriteLine(longestWord);
                Console.ReadLine();
            }
        }

        private static void BoardHistogram(Dictionary dict)
        {
            var histogram = new int[9];
            var sample = Console.ReadLine();

            for(int i=0; i < int.Parse(sample); i++)
            {
                var board = Games.Polish.GetRandomBoard();
                var layer = board.GetTopLayer();
                var longestWord = dict
                    .MatchWords(layer)
                    .Where(layer.IsAllowed)
                    .OrderBy(w => w.Length)
                    .LastOrDefault();

                if (longestWord != null)
                    histogram[longestWord.Length - 1] += 1;
            }

            for (int i=0; i<9; i++)
            {
                Console.WriteLine($"{i+1}: {histogram[i]:n0}");
            }
        }

        private static void UserInput(Dictionary dict)
        {
            while (true)
            {
                var line = Console.ReadLine();

                if (line == null)
                    return;

                //todo this is for debug:
                Console.WriteLine(line.Length);

                line = line.ToLowerInvariant();

                if (line == "q")
                    return;

                try
                {
                    Play(dict, line);
                }
                catch (Exception e)
                {
                    Console.WriteLine(" ! ERROR: " + e.Message);
                }
            }
        }

        private static void Play(Dictionary dict, string line)
        {
            var board = new BoardLayer(line.Select(g => g == ' ' ? (char?)null : g));

            var words = dict
                .MatchWords(board)
                .OrderBy(w => w.Length)
                .ToList();

            var allowed = words
                .Where(w => board.IsAllowed(w))
                .ToList();

            PrintWords(allowed);
        }

        private static void PrintWords(List<string> words)
        {
            for (int i = 0; i < words.Count - 1; i++)
            {
                Console.WriteLine(" |` " + words[i]);
            }

            Console.WriteLine("  ` " + words[words.Count - 1]);
        }

        private static void Print(Board board)
        {
            for (int i=0; i < 9; i += 3)
            {
                var a = board.GetTile(i+0)?.Glyph ?? ' ';
                var b = board.GetTile(i+1)?.Glyph ?? ' ';
                var c = board.GetTile(i+2)?.Glyph ?? ' ';
                Console.WriteLine("+-+-+-+");
                Console.WriteLine($"|{a}|{b}|{c}|");
            }

            Console.WriteLine("+-+-+-+");
        }

        private static async Task<Dictionary> LoadDictionary()
        {
            Console.WriteLine("Loading...");
            var dict = new Dictionary(Games.Polish);
            var task = dict.Load("Dictionaries/pl.txt");
            var elapsed = await MeasurementUtils.RunWithStopwatch(task);
            var compressed = 1f - dict.Words / (float) dict.Lines;
            Console.WriteLine($"Loaded in {elapsed.TotalMilliseconds} ms, lines {dict.Lines}, words {dict.Words} (compressed {compressed:P})");
            return dict;
        }
    }
}
