using System.CodeDom.Compiler;
using System.Web.Razor;
using Microsoft.CSharp;

namespace RazorPad.Compilation
{
    public class CSharpTemplateCompilationParameters : TemplateCompilationParameters
    {
        public CSharpTemplateCompilationParameters(CompilerParameters compilerParameters = null) 
            : base(new CSharpRazorCodeLanguage(), new CSharpCodeProvider(), compilerParameters)
        {
        }
    }
}