namespace Eto.Parse.Writers.Display
{
	public class UnaryWriter<T> : ParserWriter<T>
		where T: UnaryParser
	{
		public override void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
			args.Write(parser.Inner);
		}
	}
	
}
