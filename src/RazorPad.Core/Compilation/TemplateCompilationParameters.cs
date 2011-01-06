using System.CodeDom.Compiler;
using System.Web.Razor;

namespace RazorPad.Compilation
{
    public abstract class TemplateCompilationParameters
    {
        public CodeDomProvider CodeProvider { get; private set; }

        public RazorCodeLanguage Language { get; private set; }

        public CompilerParameters CompilerParameters { get; private set; }


        protected TemplateCompilationParameters(RazorCodeLanguage language, CodeDomProvider codeProvider, CompilerParameters compilerParameters = null)
        {
            Language = language;
            CodeProvider = codeProvider;
            CompilerParameters = compilerParameters ?? new CompilerParameters() { GenerateInMemory = true };
        }
    }
}