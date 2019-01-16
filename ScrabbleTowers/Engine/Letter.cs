namespace ScrabbleTowers.Engine
{
    public class Letter
    {
        public Letter(char glyph, int count)
        {
            Glyph = glyph;
            Count = count;
        }

        public char Glyph { get; }
        public int Count { get; }
    }
}