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

        string Execute(string templateText, dynamic model = null);

        GeneratorResults GenerateCode(string templateText, TextWriter codeWriter = null, RazorEngineHost host = null);

        Type GetTemplateModelType(string templateText);
    }

}