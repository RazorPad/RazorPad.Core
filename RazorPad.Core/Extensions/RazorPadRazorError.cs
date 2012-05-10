using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.Extensions
{
    public class RazorPadRazorError : RazorPadError
    {
        public RazorPadRazorError(RazorError error)
        {
            if (error == null)
                return;

            Column = error.Location.CharacterIndex;
            Line = error.Location.LineIndex;
            Message = error.Message;
        }
        
        public static implicit operator RazorPadRazorError(RazorError error)
        {
            return new RazorPadRazorError(error);
        }
    }
}