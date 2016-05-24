using System.CodeDom.Compiler;
using System.IO;
using System.Web.Razor;

namespace RazorPad.Compilation
{
    public interface ITemplateCompiler : ITemplateExecutor
    {
        TemplateCompilationParameters CompilationParameters { get; }

        CompilerResults Compile(GeneratorResults generatorResults);

        GeneratorResults GenerateCode(string templateText, TextWriter codeWriter = null, RazorEngineHost host = null);
    }

    public static class ITemplateCompilerExtensions
    {
        public static GeneratorResults GenerateCode(this ITemplateCompiler compiler, RazorDocument document, TextWriter codeWriter = null, RazorEngineHost host = null)
        {
            return compiler.GenerateCode(document.Template, codeWriter, host);
        }
    }

}