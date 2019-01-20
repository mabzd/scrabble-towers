using ScrabbleTowers.Engine;
using Xunit;

namespace ScrabbleTowers.Tests
{
    public class GameTests
    {
        [Fact]
        public void test_game()
        {
            var game = new Game("Test", new[]
            {
                new Letter('a'),
                new Letter('a'),
                new Letter('b'),
                new Letter('c')
            });

            Assert.Equal(0UL, game.GetBitword(""));
            Assert.Equal(0UL, game.GetBitword("aaaa"));
            Assert.Equal(0UL, game.GetBitword("bbb"));
            Assert.Equal(0UL, game.GetBitword("de"));

            Assert.Equal((ulong)0b00001, game.GetBitword("a"));
            Assert.Equal((ulong)0b00011, game.GetBitword("aa"));
            Assert.Equal((ulong)0b10011, game.GetBitword("aaa"));
            Assert.Equal((ulong)0b00100, game.GetBitword("b"));
            Assert.Equal((ulong)0b10100, game.GetBitword("bb"));
            Assert.Equal((ulong)0b00101, game.GetBitword("ab"));
            Assert.Equal((ulong)0b00101, game.GetBitword("ba"));
            Assert.Equal((ulong)0b00111, game.GetBitword("aba"));
            Assert.Equal((ulong)0b00111, game.GetBitword("baa"));
            Assert.Equal((ulong)0b01000, game.GetBitword("c"));
            Assert.Equal((ulong)0b01001, game.GetBitword("ac"));
            Assert.Equal((ulong)0b01001, game.GetBitword("ac"));
            Assert.Equal((ulong)0b01101, game.GetBitword("bac"));
            Assert.Equal((ulong)0b01111, game.GetBitword("caab"));
            Assert.Equal((ulong)0b11111, game.GetBitword("caabs"));
            Assert.Equal((ulong)0b10000, game.GetBitword("d"));
            Assert.Equal((ulong)0b10000, game.GetBitword("?"));
        }
    }
}
