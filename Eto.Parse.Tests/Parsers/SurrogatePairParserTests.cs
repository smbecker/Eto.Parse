using System;
using System.Linq;
using Eto.Parse.Parsers;
using Xunit;

namespace Eto.Parse.Tests.Parsers
{
    public class SurrogatePairParserTests
    {
        [Fact]
        public void TestAnySurrogatePair()
        {
            var chars = string.Format("{0},{1},{2}",
                char.ConvertFromUtf32(0x10000),
                char.ConvertFromUtf32(0x87FFF),
                char.ConvertFromUtf32(0x10FFFF));

            var grammar = new Grammar();
            var parser = new AnySurrogatePairTerminal();
            grammar.Inner = (+parser.Named("char")).SeparatedBy(",");

            var match = grammar.Match(chars);

            Assert.True(match.Success, match.ErrorMessage);
            Assert.Equal(new object[]{0x10000, 0x87FFF, 0x10FFFF}, match.Find("char").Select(parser.GetValue));
        }

        [Fact]
        public void TestMatchingSpecificSurrogatePairByCodePoint()
        {
            var sample = char.ConvertFromUtf32(0x87FFF);

            var grammar = new Grammar();
            var parser = new SingleSurrogatePairTerminal(0x87FFF);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.True(match.Success, match.ErrorMessage);
            Assert.Equal(0x87FFF, parser.GetValue(match.Find("char").Single()));
        }

        [Fact]
        public void TestUnmatchedSpecificSurrogatePairByCodePoint()
        {
            var sample = char.ConvertFromUtf32(0x17DF6);

            var grammar = new Grammar();
            var parser = new SingleSurrogatePairTerminal(0x87FFF);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.False(match.Success, match.ErrorMessage);
        }

		[Theory]
        [InlineData(0x12345)]
        [InlineData(0x57FFF)]
        [InlineData(0x8F4FE)]
        public void TestMatchingRange(int codePoint)
        {
            var sample = char.ConvertFromUtf32(codePoint);

            var grammar = new Grammar();
            var parser = new SurrogatePairRangeTerminal(0x12345, 0x8F4FE);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.True(match.Success, match.ErrorMessage);
        }

		[Theory]
        [InlineData(0x12345)]
        [InlineData(0x8F4FE)]
        public void TestMatchOutsideRange(int codePoint)
        {
            var sample = char.ConvertFromUtf32(codePoint);

            var grammar = new Grammar();
            var parser = new SurrogatePairRangeTerminal(0x12346, 0x8F4FD);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.False(match.Success, string.Format("Value {0} should be outside given range", codePoint));
        }

		[Theory]
        [InlineData(50)]
        [InlineData(0x10FFFF + 1)]
        [InlineData(-1)]
        public void TestInvaldCodePoint(int codePoint)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SingleSurrogatePairTerminal(codePoint);
                });

            Assert.Equal("codePoint", exception.ParamName);
        }

        [Fact]
        public void TestUseWithOtherParser()
        {
            var sample = "abc" + char.ConvertFromUtf32(0x8F4FE) + "def" + char.ConvertFromUtf32(0x56734);

            var grammar = new Grammar();
            var parser = new LetterTerminal() | new AnySurrogatePairTerminal();
            grammar.Inner = (+parser).Named("char");

            var match = grammar.Match(sample);

            Assert.True(match.Success, match.ErrorMessage);
            Assert.Equal(10, match.Length);
        }
    }
}