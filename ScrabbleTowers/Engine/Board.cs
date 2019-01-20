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

            public ICollection<Tile> Tiles { get; }
        }

        private readonly Stack[] Stacks = new Stack[BoardLayer.MAX_TILES];

        public Board()
        {
            for(int i = 0; i < BoardLayer.MAX_TILES; i++)
                Stacks[i] = new Stack();
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