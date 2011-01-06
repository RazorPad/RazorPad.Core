using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace RazorPad.Compilation.EmbeddedResources
{
    public class RazorViewComponentAssemblyCompilationException : Exception
    {
        public IEnumerable<CompilerResults> CompiledAssemblies { get; private set; }

        public RazorViewComponentAssemblyCompilationException(IEnumerable<CompilerResults> compiledAssemblies)
        {
            CompiledAssemblies = compiledAssemblies ?? Enumerable.Empty<CompilerResults>();
        }

        public override string Message
        {
            get
            {
                return "There were errors compiling embedded Razor templates:\r\n" +
                       string.Join("\r\n", 
                                   CompiledAssemblies
                                       .SelectMany(x => x.Errors.Cast<CompilerError>())
                                       .Select(FormatCompilerError)
                           );
            }
        }

        private static string FormatCompilerError(CompilerError compilerError)
        {
            return string.Format(
                "{0}, Line {1}: {2}",
                compilerError.FileName,
                compilerError.Line,
                compilerError.ErrorText
                );
        }
    }
}