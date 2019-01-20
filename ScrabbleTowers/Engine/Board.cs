using System.Collections.Generic;
using System.Linq;

namespace ScrabbleTowers.Engine
{
    public class Board
    {
        private class Stack
        {
            public Stack(IEnumerable<Tile> tiles = null)
            {
                Tiles = tiles?.ToList() ?? new List<Tile>();
            }

            public IList<Tile> Tiles { get; }
        }

        private readonly Stack[] Stacks = new Stack[BoardLayer.MAX_TILES];

        public Board()
        {
            for(int i = 0; i < BoardLayer.MAX_TILES; i++)
                Stacks[i] = new Stack();
        }

        public Tile GetTile(int x, int y)
        {
            return GetTile(Tile.ToPosition(x, y));
        }

        public Tile GetTile(int position)
        {
            return Stacks[position].Tiles.LastOrDefault();
        }

        public void RemoveTile(int x, int y)
        {
            RemoveTile(Tile.ToPosition(x, y));
        }

        public void RemoveTile(int position)
        {
            var count = Stacks[position].Tiles.Count;
            if (count > 0)
                Stacks[position].Tiles.RemoveAt(count - 1);
        }

        public void AddStack(int x, int y, IEnumerable<Letter> letters)
        {
            AddStack(Tile.ToPosition(x, y), letters);
        }

        public void AddStack(int position, IEnumerable<Letter> letters)
        {
            var tiles = letters?.Select(l => new Tile(l.Glyph, position));
            var stack = new Stack(tiles);
            Stacks[position] = stack;
        }

        public BoardLayer GetTopLayer()
        {
            var tiles = GetTopTiles();
            return new BoardLayer(tiles);
        }

        private IEnumerable<Tile> GetTopTiles()
        {
            for(int i = 0; i < BoardLayer.MAX_TILES; i++)
            {
                var tile = Stacks[i].Tiles.LastOrDefault();
                if (tile != null)
                    yield return tile;
            }
        }
    }
}