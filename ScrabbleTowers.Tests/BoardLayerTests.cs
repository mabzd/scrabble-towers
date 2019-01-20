using ScrabbleTowers.Engine;
using Xunit;

namespace ScrabbleTowers.Tests
{
    public class BoardLayerTests
    {
        private readonly char?[] Glyphs = {
            'o', 'ł', 'a',
            'z', null, 'i',
            'j', 'd', 'o'
        };

        [Fact]
        public void test_allowed_word()
        {
            var board = new BoardLayer(Glyphs);

            var allowed = board.IsAllowed("dział");

            Assert.True(allowed);
        }

        [Fact]
        public void test_disallowed_word()
        {
            var board = new BoardLayer(Glyphs);

            var disallowed = board.IsAllowed("ozdobiły");

            Assert.False(disallowed);
        }
    }
}
