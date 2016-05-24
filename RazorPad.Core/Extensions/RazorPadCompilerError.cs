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

        public override string ToString()
        {
            return string.Format("[{0}:{1}] {2}", Line, Column, Message);
        }
    }
}