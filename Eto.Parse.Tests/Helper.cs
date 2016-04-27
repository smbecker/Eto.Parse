using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace Eto.Parse.Tests
{
	public static class Helper
	{
#if !CORECLR
		public static T Create<T>(string code, string className, params string[] referencedAssemblies)
		{
			using (var csharp = new Microsoft.CSharp.CSharpCodeProvider())
			{
				var parameters = new System.CodeDom.Compiler.CompilerParameters()
				{
					GenerateInMemory = true,
				};
				if (referencedAssemblies != null)
					parameters.ReferencedAssemblies.AddRange(referencedAssemblies);
				var assemblyName = typeof(T).Assembly.Location;
				if (typeof(T).Assembly != Assembly.GetExecutingAssembly() && (referencedAssemblies == null || !referencedAssemblies.Contains(assemblyName)))
				{
					parameters.ReferencedAssemblies.Add(assemblyName);
				}

				Console.WriteLine(string.Join(", ", parameters.ReferencedAssemblies.Cast<string>()));

				var res = csharp.CompileAssemblyFromSource(parameters, code);
				if (res.Errors.HasErrors)
				{
					var errors = string.Join("\n", res.Errors.OfType<System.CodeDom.Compiler.CompilerError>().Select(r => r.ToString()));
					throw new Exception(string.Format("Error compiling:\n{0}", errors));
				}

				return (T)res.CompiledAssembly.CreateInstance(className);
			}
		}
#endif

		public static void TestSpeed(Grammar grammar, string input, int iterations)
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterations; i++)
			{
				grammar.Match(input);
			}
			sw.Stop();
			Console.WriteLine("{0} seconds for {1} iterations", sw.Elapsed.TotalSeconds, iterations);
		}
	}
}

