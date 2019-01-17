using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ScrabbleTowers.Engine
{
    public class Board : IEnumerable<Tile>
    {
        private struct Direction
        {
            public Direction(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        private const int MAX_TILES = 9;
        private const int WILDCARD_POSITION = MAX_TILES / 2;
        private readonly ICollection<Tile> Tiles;

        private readonly ICollection<Direction> Directions = new[]
        {
            new Direction(1, 0),
            new Direction(1, -1),
            new Direction(0, -1),
            new Direction(-1, -1),
            new Direction(-1, 0),
            new Direction(-1, 1),
            new Direction(0, 1),
            new Direction(1, 1),
        };

        public Board(IEnumerable<char?> glyphs)
        {
            var tiles = CreateTiles(glyphs).ToList();
            LinkTiles(tiles);
            Tiles = tiles.Where(t => t != null).ToList();
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        public bool IsAllowed(string word)
        {
            return Traverse(Tiles, 0, word);
        }

        private bool Traverse(ICollection<Tile> neighbors, int visited, string word)
        {
            var glyph = word[0];
            word = word.Substring(1);

            foreach (var neighbor in neighbors)
            {
                var bit = 1 << neighbor.Position;
                var isVisited = (visited & bit) == bit;

                if (!isVisited && (neighbor.Glyph == null || neighbor.Glyph.Value == glyph))
                {
                    if (word == "")
                        return true;

                    if (Traverse(neighbor.Neighbors, visited | bit, word))
                        return true;
                }
            }

            return false;
        }

        private IEnumerable<Tile> CreateTiles(IEnumerable<char?> glyphs)
        {
            var position = 0;

            foreach (var glyph in glyphs)
            {
                if (glyph == null)
                {
                    yield return position == WILDCARD_POSITION
                        ? new Tile(WILDCARD_POSITION, null)
                        : null;
                }
                else
                {
                    yield return new Tile(position, glyph.Value);
                }

                position += 1;
            }

            if (position != MAX_TILES)
                throw new InvalidOperationException($"Board must consist of {MAX_TILES} tiles");
        }

        private void LinkTiles(IList<Tile> tiles)
        {
            for (int position = 0; position < MAX_TILES; position++)
            {
                var tile = tiles[position];

                if (tile != null)
                {
                    foreach (var direction in Directions)
                    {
                        var neighbor = GetNeighbor(tiles, position, direction);

                        if (neighbor != null)
                            tile.Neighbors.Add(neighbor);
                    }
                }
            }
        }

        private (int, int) PositionToXy(int position)
        {
            var x = position % 3;
            var y = position / 3;
            return (x, y);
        }

        private int XyToPosition(int x, int y)
        {
            return y * 3 + x;
        }

        private Tile GetNeighbor(IList<Tile> tiles, int position, Direction direction)
        {
            var (x, y) = PositionToXy(position);

            x += direction.X;
            y += direction.Y;

            if (x < 0 || x > 2 || y < 0 || y > 2)
                return null;

            var newPosition = XyToPosition(x, y);
            return tiles[newPosition];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
