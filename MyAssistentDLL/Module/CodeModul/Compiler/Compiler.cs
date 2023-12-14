using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace MyAssistentDLL.Module.CodeModul.Compiler
{
    internal class CodeCompiler
    {
        
        public static bool Compile(string code, out List<ErrorInfo> Error, out System.IO.MemoryStream ms)
        {
            Error = new List<ErrorInfo>();
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var compilation = CSharpCompilation.Create("MyCompilation")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(AsisstentCodeBase).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location))
                .AddSyntaxTrees(syntaxTree);


            ms = new System.IO.MemoryStream();

            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {

                foreach (var diagnostic in result.Diagnostics)
                {

                    var lineSpan = diagnostic.Location.GetLineSpan();
                    Error.Add(new ErrorInfo(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character,
                        diagnostic.Id, diagnostic.GetMessage()));

                }

            }
            return result.Success;

        }
        public static void Run(string code)
        {


            if (Compile(code, out var err, out var ms))
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                var scriptRunnerType = assembly.GetType("Program");
                var executeCodeMethod = scriptRunnerType.GetMethod("Main");

                executeCodeMethod.Invoke(null, null);
                ms.Close();
            }
            else
            {
                foreach (var i in err)
                    Console.WriteLine(i.ToString());
            }
        }
        public static void Run(System.IO.MemoryStream ms)
        {
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());

            var scriptRunnerType = assembly.GetType("Program");
            var executeCodeMethod = scriptRunnerType.GetMethod("Main");

            executeCodeMethod.Invoke(null, null);
            ms.Close();
        }
    }
}
