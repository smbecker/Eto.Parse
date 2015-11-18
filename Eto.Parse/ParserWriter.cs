using System;
using System.Collections.Generic;
#if CORECLR
using System.Reflection;
#endif

namespace Eto.Parse
{
	public interface IParserWriter
	{
		string WriteParser(ParserWriterArgs args, Parser parser);
	}

	public class ParserWriter : ParserWriter<ParserWriterArgs>
	{
		public ParserWriter(ParserDictionary writers)
			: base(writers)
		{
		}

		public ParserWriter()
		{
		}
	}

	public class ParserWriter<TArgs> : IParserWriter
		where TArgs: ParserWriterArgs
	{
		public interface IParserWriterHandler
		{
			string Write(TArgs args, Parser parser);
		}

		public class ParserDictionary : Dictionary<Type, IParserWriterHandler> { }

		public ParserDictionary ParserWriters { get; private set; }

		public ParserWriter(ParserDictionary writers = null)
		{
			ParserWriters = writers ?? new ParserDictionary();
		}

		public virtual string WriteParser(TArgs args, Parser parser)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");
			var type = parser.GetType();
			while (type != null)
			{
				IParserWriterHandler handler;
				if (ParserWriters.TryGetValue(type, out handler))
					return handler.Write(args, parser);
#if CORECLR
				type = type.GetTypeInfo().BaseType;
#else
				type = type.BaseType;
#endif
			}
			return null;
		}

		string IParserWriter.WriteParser(ParserWriterArgs args, Parser parser)
		{
			return WriteParser((TArgs)args, parser);
		}
	}
}

