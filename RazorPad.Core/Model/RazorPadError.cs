using System;
using System.CodeDom.Compiler;
using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.Model
{
    public class RazorPadError
    {
        public int Line { get; set; }

        public int Column { get; set; }

        public string Message { get; set; }

        public RazorPadError(string message = null)
        {
            Message = message;
        }

        public RazorPadError(RazorError error)
        {
            if (error == null)
                return;

            Column = error.Location.CharacterIndex;
            Line = error.Location.LineIndex;
            Message = error.Message;
        }

        public RazorPadError(CompilerError error)
        {
            if (error == null)
                return;

            Column = error.Column;
            Line = error.Line;
            Message = error.ErrorText;
        }

        public RazorPadError(Exception exception)
        {
            if (exception == null)
                return;

            Message = string.Format("EXCEPTION: {0}", exception.Message);
        }


        public static implicit operator RazorPadError(RazorError error)
        {
            return new RazorPadError(error);
        }

        public static implicit operator RazorPadError(CompilerError error)
        {
            return new RazorPadError(error);
        }

        public static implicit operator RazorPadError(Exception exception)
        {
            return new RazorPadError(exception);
        }
    }
}