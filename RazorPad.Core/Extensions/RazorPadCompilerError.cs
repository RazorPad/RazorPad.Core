using System.CodeDom.Compiler;

namespace RazorPad.Extensions
{
    public class RazorPadCompilerError : RazorPadError
    {
        public RazorPadCompilerError(CompilerError error)
        {
            if (error == null)
                return;

            Column = error.Column;
            Line = error.Line;
            Message = error.ErrorText;
        }

        public static implicit operator RazorPadCompilerError(CompilerError error)
        {
            return new RazorPadCompilerError(error);
        }
    }
}