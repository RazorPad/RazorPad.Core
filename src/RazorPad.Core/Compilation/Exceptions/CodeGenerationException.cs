using System;
using System.Web.Razor;

namespace RazorPad.Compilation
{
    public class CodeGenerationException : Exception
    {
        public GeneratorResults GeneratorResults { get; set; }

        public CodeGenerationException(GeneratorResults generatorResults)
            : base("The template could not be parsed")
        {
            GeneratorResults = generatorResults;
        }
    }
}