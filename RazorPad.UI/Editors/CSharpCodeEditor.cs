using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace RazorPad.UI.Editors
{
    public class CSharpCodeEditor : CodeEditor
    {
        public CSharpCodeEditor()
        {
            Editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(Editor.Options);
            InitializeFolding(new BraceFoldingStrategy());
            Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
        }
    }

}
