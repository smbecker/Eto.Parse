using Eto.Parse.Parsers;
using System.Linq;
using Xunit;

namespace Eto.Parse.Tests.Parsers
{
	public class StringParserTests
	{
		[Fact]
		public void TestQuoting()
		{
			var sample = "string1,\"string 2\",'string 3'";

			var grammar = new Grammar();
			var str = new StringParser { AllowNonQuoted = true };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new string[] { "string1", "string 2", "string 3" }, match.Find("str").Select(m => str.GetValue(m)));
		}

		[Fact]
		public void TestEscaping()
		{
			var sample = "\"string\\'\\\"\\0\\a\\b\\f\\n\\r\\t\\v\\x123\\u1234\\U00001234 1\",'string\\'\\\"\\0\\a\\b\\f\\n\\r\\t\\v\\x123\\u1234\\U00001234 2'";

			var grammar = new Grammar();
			var str = new StringParser { AllowEscapeCharacters = true  };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			var values = match.Find("str").Select(m => str.GetValue(m)).ToArray();
			Assert.Equal(new object[] { "string\'\"\0\a\b\f\n\r\t\v\x123\u1234\U00001234 1", "string\'\"\0\a\b\f\n\r\t\v\x123\u1234\U00001234 2" }, values);
		}

		[Fact]
		public void TestDoubleQuoting()
		{
			var sample = "\"string\"\" ''1'\",'string'' \"\"2\"'";

			var grammar = new Grammar();
			var str = new StringParser { AllowDoubleQuote = true };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new string[] { "string\" ''1'", "string' \"\"2\"" }, match.Find("str").Select(m => str.GetValue(m)));
		}

		[Fact]
		public void TestErrorConditionsNoOptions()
		{
			var samples = new string[] { "string1", "\"string 2", "'string 3", "'string ''4", "string5'", "string6\"", "\"string\\\"7\"", "string 8" };

			var grammar = new Grammar();
			var str = new StringParser { AllowDoubleQuote = false, AllowNonQuoted = false, AllowEscapeCharacters = false };

			grammar.Inner = str.Named("str");

			foreach (var sample in samples)
			{
				var match = grammar.Match(sample);
				Assert.False(match.Success, string.Format("Should not match string {0}", sample));
			}
		}

		[Fact]
		public void TestErrorConditionsWithOptions()
		{
			var samples = new string[] { "string 1", "\"string 2", "'string 3", "'string ''4", "string5'", "string6\"", "\"string\\\"7" };

			var grammar = new Grammar();
			var str = new StringParser { AllowDoubleQuote = true, AllowNonQuoted = true, AllowEscapeCharacters = true };

			grammar.Inner = str.Named("str");

			foreach (var sample in samples)
			{
				var match = grammar.Match(sample);
				Assert.False(match.Success, string.Format("Should not match string {0}", sample));
			}
		}
	}
}

