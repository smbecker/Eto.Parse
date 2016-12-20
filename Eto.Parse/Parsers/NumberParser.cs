using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Eto.Parse.Parsers
{
	public class NumberParser : Parser
	{
		MethodInfo parseMethod;

		public bool AllowSign { get; set; }

		public bool AllowDecimal { get; set; }

		public char DecimalSeparator { get; set; }

		public bool AllowExponent { get; set; }

		public Type ValueType { get; set; }

		protected NumberParser(NumberParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			AllowSign = other.AllowSign;
			AllowExponent = other.AllowExponent;
			DecimalSeparator = other.DecimalSeparator;
			ValueType = other.ValueType;
		}

		public NumberParser()
		{
			DecimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			ValueType = typeof(decimal);
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			if (args.Push(this))
			{
				if (ValueType != null) {
#if CORECLR
					parseMethod = ValueType.GetTypeInfo().GetDeclaredMethods("Parse")
							.FirstOrDefault(x => {
								if (!x.IsStatic || !x.IsPublic) {
									return false;
								}
								var parameters = x.GetParameters();
								return parameters.Length == 2
									&& parameters[0].ParameterType == typeof(string)
									&& parameters[1].ParameterType == typeof(NumberStyles);
							});
#else
					parseMethod = ValueType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(NumberStyles) }, null);
#endif
				}
				args.Pop();
			}
		}

		public override object GetValue(string text)
		{
			var style = NumberStyles.None;

			if (AllowSign)
				style |= NumberStyles.AllowLeadingSign;
			if (AllowDecimal)
				style |= NumberStyles.AllowDecimalPoint;
			if (AllowExponent)
				style |= NumberStyles.AllowExponent;

			return parseMethod.Invoke(null, new object[] { text, style });
		}

		protected override int InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var len = 0;
			int ch;
			var pos = scanner.Position;
			if (AllowSign)
			{
				ch = scanner.ReadChar();
				if (ch == -1)
				{
					return -1;
				}
				if (ch == '-' || ch == '+')
				{
					len++;
					ch = scanner.ReadChar();
				}
			}
			else
				ch = scanner.ReadChar();

			bool foundNumber = false;
			bool hasDecimal = false;
			bool hasExponent = false;
			do
			{
				if (char.IsDigit((char)ch))
				{
					foundNumber = true;
				}
				else if (AllowDecimal && !hasDecimal && ch == DecimalSeparator)
				{
					hasDecimal = true;
				}
				else if (hasExponent && (ch == '+' || ch == '-'))
				{
				}
				else if (AllowExponent && !hasExponent && (ch == 'E' || ch == 'e'))
				{
					hasExponent = true;
					hasDecimal = true; // no decimals after exponent
				}
				else if (!foundNumber)
				{
					scanner.Position = pos;
					return -1;
				}
				else
					break;
				len++;
				ch = scanner.ReadChar();
			}
			while (ch != -1);
			scanner.Position = pos + len;
			return len;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new NumberParser(this, args);
		}
	}
}

