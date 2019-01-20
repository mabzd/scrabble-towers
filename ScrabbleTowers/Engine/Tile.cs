using System.Collections.Generic;

namespace ScrabbleTowers.Engine
{

    public class Tile
    {
        public Tile(char glyph, int position, LetterColor color = LetterColor.Transparent)
        {
            Glyph = glyph;
            Position = position;
            Color = color;
        }

        public char Glyph { get; }
        public int Position { get;}
        public LetterColor Color { get; }

        public (int, int) ToXy()
        {
            return ToXy(Position);
        }

        public static (int, int) ToXy(int position)
        {
            var x = position % 3;
            var y = position / 3;
            return (x, y);
        }

        public static int ToPosition(int x, int y)
        {
            return y * 3 + x;
        }
    }
}