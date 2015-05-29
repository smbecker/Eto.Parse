using System.IO;
#if DNXCORE50
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
#if DNXCORE50
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
#if DNXCORE50
				return 0;
#else
				return Output.Indent;
#endif
			}
			set 
			{
#if !DNXCORE50
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
