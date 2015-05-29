using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse.Writers.Code
{
	public class ListWriter<T> : ParserWriter<T>
		where T: ListParser
	{
		public override void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
			base.WriteContents(args, parser, name);
			var items = new List<string>(parser.Items.Select(r => r != null ? args.Write(r) : "null"));
			args.Output.WriteLine("{0}.Items.AddRange(new Eto.Parse.Parser[] {{ {1} }});", name, string.Join(", ", items));
		}
	}
	
}
