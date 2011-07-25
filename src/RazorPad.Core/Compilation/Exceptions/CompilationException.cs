using System;
using System.CodeDom.Compiler;

namespace RazorPad.Compilation
{
    public class CompilationException : Exception
    {
        public CompilerResults CompilerResults { get; set; }

        public CompilationException(CompilerResults compilerResults)
            : base("There were errors compiling the template")
        {
            CompilerResults = compilerResults;
        }
    }
}