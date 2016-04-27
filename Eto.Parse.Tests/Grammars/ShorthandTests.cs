using Xunit;

namespace Eto.Parse.Tests.Grammars
{
	public class ShorthandTests
	{
		[Fact]
		public void Simple()
		{
			var input = "  hello ( parsing world )  ";

			// optional repeating whitespace
			var ws = -Terminals.WhiteSpace;

			// parse a value with or without brackets
			Parser valueParser = 
				('(' & ws & (+Terminals.AnyChar ^ (ws & ')')).Named("value") & ws & ')')
				| (+!Terminals.WhiteSpace).Named("value");

			// our grammar
			var grammar = new Grammar(ws & valueParser.Named("first") & ws & valueParser.Named("second") & ws & Terminals.End);

			var match = grammar.Match(input);
			Assert.True(match.Success);
			Assert.Equal("hello", match["first"]["value"].Text);
			Assert.Equal("parsing world", match["second"]["value"].Text);
		}

		[Fact]
		public void RepeatUntil()
		{
			var input = "abc def 1234";

			// optional repeating whitespace
			var ws = -Terminals.WhiteSpace;

			// repeat until we get a digit, and exclude any whitespace inbetween
			var repeat = +Terminals.AnyChar ^ (ws & Terminals.Digit);

			var match = new Grammar(repeat) { AllowPartialMatch = true }.Match(input);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal("abc def", match.Text);
		}
	}
}

