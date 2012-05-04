using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.Compilation
{
    public static class ErrorExtensions
    {

        public static IEnumerable<string> Format(this IEnumerable<RazorError> errors)
        {
            var formatted = errors.Select(error => 
                string.Format("[{0}] {1}", error.Location, error.Message));
            return formatted;
        }

        public static string Render(this IEnumerable<RazorError> errors)
        {
            var formatted = Format(errors);
            return string.Join("\r\n", formatted);
        }

        public static IEnumerable<string> Format(this CompilerErrorCollection errors)
        {
            return from CompilerError error in errors 
                   select string.Format("{0}: [{1}:{2}] {3}",
                            error.IsWarning ? "Warning" : "ERROR",
                            error.Line, error.Column,
                            error.ErrorText);
        }

        public static string Render(this CompilerErrorCollection errors)
        {
            var formatted = Format(errors);
            return string.Join("\r\n", formatted);
        }

    }
}
