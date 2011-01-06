using System.CodeDom.Compiler;
using System.Web.Razor;
using Microsoft.VisualBasic;

namespace RazorPad.Compilation
{
    public class VBTemplateCompilationParameters : TemplateCompilationParameters
    {
        public VBTemplateCompilationParameters(CompilerParameters compilerParameters = null) 
            : base(new VBRazorCodeLanguage(), new VBCodeProvider(), compilerParameters)
        {
        }
    }
}