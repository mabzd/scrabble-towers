namespace ScrabbleTowers.Utils
{
    public static class BitUtils
    {
        /// <summary>
        /// Source: https://stackoverflow.com/questions/6097635/checking-cpu-popcount-from-c-sharp
        /// </summary>
        public static int CountSetBits(ulong value)
        {
            ulong result = value - ((value >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (int)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }
    }
}
