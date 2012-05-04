using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace RazorPad.UI.CodeEditors
{
    public class CSharpCodeEditor : CodeEditor
    {
        public CSharpCodeEditor()
        {
            TextArea.IndentationStrategy = new CSharpIndentationStrategy(Options);
            InitializeFolding(new BraceFoldingStrategy());
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
        }
    }
}