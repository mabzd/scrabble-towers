namespace ScrabbleTowers.Engine
{
    public class Word
    {
        public Word(string value, ulong bits)
        {
            Value = value;
            Bits = bits;
        }

        public string Value { get; }
        public ulong Bits { get; }
    }
}