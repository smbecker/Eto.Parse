namespace Eto.Parse.Writers.Display
{
	public class ListWriter : ParserWriter<ListParser>
	{
		public override void WriteContents(TextParserWriterArgs args, ListParser parser, string name)
		{
			foreach (var r in parser.Items) 
			{
				args.Write(r);
			}
		}
	}
	
}
