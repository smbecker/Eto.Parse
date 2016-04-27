using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Eto.Parse.Samples.Markdown;
using Xunit;

namespace Eto.Parse.Tests.Markdown
{
	public class MarkdownTests
	{
		private static readonly List<string> markdownResources = typeof(MarkdownTests)
			.GetTypeInfo()
			.Assembly.GetManifestResourceNames()
			.Where(x => x.IndexOf("Markdown", StringComparison.OrdinalIgnoreCase) != -1)
			.ToList();
		private readonly MarkdownGrammar grammar = new MarkdownGrammar();

		[Theory(Skip = "Do not run for now")]
		[MemberData("GetAllTests")]
		public void ExecuteTest(string name)
		{
			//ExecuteTest(name, deep.Transform);
			ExecuteTest(name, grammar.Transform);
		}

		public void ExecuteTest(string name, Func<string, string> generate) 
		{
			var textResource = markdownResources.FirstOrDefault(x => x == name + ".text");
			if (textResource == null)
			{
				textResource = markdownResources.FirstOrDefault(x => x == name + ".txt");
			}

			Assert.False(textResource == null, string.Format("Could not locate {0}.txt", name));

			var htmlResource = markdownResources.FirstOrDefault(x => x == name + ".html");
			Assert.False(htmlResource == null, string.Format("Could not locate {0}.html", name));

			var text = ReadTextFromResource(textResource);
			var html = ReadTextFromResource(htmlResource);

			var generatedHtml = generate(text);
			//Console.WriteLine(generatedHtml);
			CompareHtml(html, generatedHtml);
		}

		private static string ReadTextFromResource(string resourceName)
		{
			using (var stream = typeof(MarkdownTests).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName))
			{
				return new StreamReader(stream).ReadToEnd();
			}
		}

		private static void CompareHtml(string html, string generatedHtml)
		{
			Assert.Equal(RemoveNewlines(html), RemoveNewlines(generatedHtml));
			/**
			var constraint = Is.EqualTo(generatedHtml);
			if (!constraint.Matches(html))
			{
				var writer = new TextMessageWriter("Whitespace is different");
				constraint.WriteMessageTo(writer);
				Assert.Inconclusive(writer.ToString());
			}
			/**/
		}

		private static string RemoveNewlines(string html)
		{
			html = html.Replace("\r\n", "\n");
			html = html.Replace("\r", "\n");
			html = html.Replace("\n", " ");
			html = html.Replace("\t", " ");
			// ignore multiple newlines
			//html = Regex.Replace(html, "((?<=[>])[ ]+)?[\\n]+([ ]{0,3}(?=[<]))?", "\n", RegexOptions.Compiled);
			// ignore space between two tags
			html = Regex.Replace(html, "[>]\\s+[<]", "><", RegexOptions.Compiled);
			// ignore whitespace before beginning/ending tag
			html = Regex.Replace(html, "[\\n ]+<", "<", RegexOptions.Compiled);
			// ignore whitespace after start tag
			html = Regex.Replace(html, ">[\\n ]+", ">", RegexOptions.Compiled);
			// ignore space after newline
			html = Regex.Replace(html, "[\\n ]+", " ", RegexOptions.Compiled);
			// ignore tabs
			return html.Trim();
		}

		public static IEnumerable<object[]> GetAllTests() {
			return new[] {
				GetTests("blocktests"),
				// GetTests("extramode"),
				GetTests("mdtest01"),
				GetTests("mdtest11"),
				//GetTests("pandoc"), // other parsers don't pass these either
				GetTests("phpmarkdown"),
				GetTests("safemode"),
				GetTests("simple"),
				GetTests("spantests"),
				GetTests("xsstests")
			}.SelectMany(x => x.ToList())
			.Select(x => new object[] { x });
		}

		private static IEnumerable<string> GetTests(string path)
		{
			return markdownResources.Where(x => x.IndexOf(path, StringComparison.OrdinalIgnoreCase) != -1)
				.Where(x => x.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
				.Select(Path.GetFileNameWithoutExtension)
				.Distinct();
		}
	}
}

