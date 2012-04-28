using System.CodeDom.Compiler;
using System.IO;

namespace RazorPad.Extensions
{
    public static class CompilerErrorCollectionExtensions
    {

        public static void Trace(this CompilerErrorCollection errors)
        {
            if(errors == null || !(errors.HasErrors && errors.HasWarnings))
                return;

            foreach (CompilerError error in errors)
            {
                if(error.IsWarning)
                    System.Diagnostics.Trace.TraceWarning(error.LogMessage());
                else
                    System.Diagnostics.Trace.TraceError(error.LogMessage());
            }
        }

        public static void LogTo(this CompilerErrorCollection errors, TextWriter writer)
        {
            foreach (CompilerError error in errors)
                writer.WriteLine(error.LogMessage());
        }

        public static string LogMessage(this CompilerError error)
        {
            return string.Format("[{0}] Line {1} {2}: {3}", 
                                 error.ErrorNumber, error.Line, error.Column, error.ErrorText);
        }

    }
}
