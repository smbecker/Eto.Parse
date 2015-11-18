using System.IO;
#if CORECLR
using OutputWriter = System.IO.TextWriter;
#else
using OutputWriter = System.CodeDom.Compiler.IndentedTextWriter;
#endif

namespace Eto.Parse
{
	public class TextParserWriterArgs : ParserWriterArgs
	{
		public TextParserWriterArgs(TextWriter writer, string indent) 
		{
#if CORECLR
			Output = writer;
#else
			Output = new OutputWriter(writer, indent);
#endif
		}

		public OutputWriter Output { get; private set; }

		public override int Level
		{
			get 
			{
#if CORECLR
				return 0;
#else
				return Output.Indent;
#endif
			}
			set 
			{
#if !CORECLR
				Output.Indent = value;
#endif
			}
		}

		public new TextParserWriter Writer
		{ 
			get { return (TextParserWriter)base.Writer; }
			set { base.Writer = value; } 
		}
	}
}
