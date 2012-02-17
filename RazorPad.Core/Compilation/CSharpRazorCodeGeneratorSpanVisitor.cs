using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.Compilation
{
    public abstract class CSharpRazorCodeGeneratorSpanVisitor
    {
        public abstract bool TryVisit(Span span);
    }
}