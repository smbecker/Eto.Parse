namespace Eto.Parse.Writers.Code
{
	public class GrammarWriter : UnaryWriter<Grammar>
	{
		bool writeNewObject;
		protected override bool WriteNewObject { get { return writeNewObject; } }

		public override string GetName(TextParserWriterArgs args, Grammar parser)
		{
			var writer = args.Writer as CodeParserWriter;
			if (!string.IsNullOrEmpty(writer.ClassName))
				return "this";
			else
				return GetIdentifier(parser.Name);
		}

		public override void WriteObject(TextParserWriterArgs args, Grammar parser, string name)
		{
			var writer = args.Writer as CodeParserWriter;
			if (!string.IsNullOrEmpty(writer.ClassName))
			{
				writeNewObject = false;
				var iw = args.Output;
				iw.WriteLine("public class {0} : Eto.Parse.Grammar", writer.ClassName);
				iw.WriteLine("{");
#if !CORECLR
				iw.Indent ++;
#endif

				iw.WriteLine("public {0}()", writer.ClassName);
#if !CORECLR
				iw.Indent ++;
#endif
				iw.WriteLine(": base(\"{0}\")", parser.Name.Replace("\"", "\\\""));
#if !CORECLR
				iw.Indent --;
#endif
				iw.WriteLine("{");
#if !CORECLR
				iw.Indent ++;
#endif

				base.WriteObject(args, parser, name);
			}
			else
			{
				writeNewObject = true;
				base.WriteObject(args, parser, name);
				args.Output.WriteLine("{0}.Name = \"{1}\";", name, parser.Name);
			}
		}

		public override void WriteContents(TextParserWriterArgs args, Grammar parser, string name)
		{
			base.WriteContents(args, parser, name);
			var writer = args.Writer as CodeParserWriter;
			if (!string.IsNullOrEmpty(writer.ClassName))
			{
				var iw = args.Output;
#if !CORECLR
				iw.Indent --;
#endif
				iw.WriteLine("}");
#if !CORECLR
				iw.Indent --;
#endif
				iw.WriteLine("}");
			}
		}
	}
	
}
