using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.CodeDom.Compiler;
using System.Web.Razor;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Reflection;

namespace RazorPad.Compilation
{
    public class TemplateCompilationParameters
    {
        public static TemplateCompilationParameters CSharp
        {
            get { return new TemplateCompilationParameters(new CSharpRazorCodeLanguage(), new CSharpCodeProvider()); }
        }

        public static TemplateCompilationParameters VisualBasic
        {
            get { return new TemplateCompilationParameters(new VBRazorCodeLanguage(), new VBCodeProvider()); }
        }

        public CodeDomProvider CodeProvider { get; private set; }

        public RazorCodeLanguage Language { get; private set; }

        public CompilerParameters CompilerParameters { get; private set; }


        protected TemplateCompilationParameters(RazorCodeLanguage language, CodeDomProvider codeProvider, CompilerParameters compilerParameters = null)
        {
            Language = language;
            CodeProvider = codeProvider;
            CompilerParameters = compilerParameters ?? new CompilerParameters { GenerateInMemory = true };
            AddAssemblyReference(typeof(TemplateBase));
        }


        public void AddAssemblyReference(Type type)
        {
            Contract.Requires(type != null);

            AddAssemblyReference(type.Assembly.Location);
        }

        public void AddAssemblyReference(Assembly assembly)
        {
            Contract.Requires(assembly != null);

            AddAssemblyReference(assembly.Location);
        }

        public void AddAssemblyReference(string location)
        {
            Contract.Requires(string.IsNullOrWhiteSpace(location) == false);

            CompilerParameters.ReferencedAssemblies.Add(location);
        }


        public static TemplateCompilationParameters CreateFromFilename(string filename)
        {
            if (filename.EndsWith(".vbhtml", StringComparison.OrdinalIgnoreCase))
                return VisualBasic;

            return CSharp;
        }

        public static TemplateCompilationParameters CreateFromLanguage(TemplateLanguage language)
        {
            switch (language)
            {
                case(TemplateLanguage.VisualBasic):
                    return VisualBasic;
                
                default:
                    return CSharp;
            }
        }
    }
}