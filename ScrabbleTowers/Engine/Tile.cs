using System.Collections.Generic;

namespace ScrabbleTowers.Engine
{
    public class Tile
    {
        public Tile(int position, char? glyph)
        {
            Glyph = glyph;
            Position = position;
            Neighbours = new List<Tile>(8);
        }

        public char? Glyph { get; }
        public int Position { get; }
        public ICollection<Tile> Neighbours { get; }
    }
}