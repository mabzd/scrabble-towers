using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var dict = await LoadDictionary();

            while (true)
            {
                var line = Console.ReadLine();

                if (line == null)
                    return;

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
            var board = new Board(line.Select(g => g == ' ' ? (char?)null : g));

            var words = dict
                .MatchWords(line)
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
