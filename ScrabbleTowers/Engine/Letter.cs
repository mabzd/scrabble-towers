namespace ScrabbleTowers.Engine
{
    public class Letter
    {
        public Letter(char glyph, LetterColor color = LetterColor.Transparent)
        {
            Glyph = glyph;
            Color = color;
        }

        public char Glyph { get; }
        public LetterColor Color { get; }
    }
}