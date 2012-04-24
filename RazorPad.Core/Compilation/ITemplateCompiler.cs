using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Razor;

namespace RazorPad.Compilation
{

    public interface ITemplateCompiler
    {
        TemplateCompilationParameters CompilationParameters { get; }

        CompilerResults Compile(string templateText);
        CompilerResults Compile(GeneratorResults generatorResults);

        string Execute(string templateText, dynamic model = null, RazorEngineHost host = null);

        GeneratorResults GenerateCode(string templateText, TextWriter codeWriter = null, RazorEngineHost host = null);

        Type GetTemplateModelType(string templateText);
    }

    public static class ITemplateCompilerExtensions
    {
        public static CompilerResults Compile(this ITemplateCompiler compiler, RazorDocument document)
        {
            return compiler.Compile(document.Template);
        }

        public static string Execute(this ITemplateCompiler compiler, RazorDocument document, RazorEngineHost host = null)
        {
            return compiler.Execute(document.Template, document.GetModel(), host);
        }

        public static GeneratorResults GenerateCode(this ITemplateCompiler compiler, RazorDocument document, TextWriter codeWriter = null, RazorEngineHost host = null)
        {
            return compiler.GenerateCode(document.Template, codeWriter, host);
        }
    }

}