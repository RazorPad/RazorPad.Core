using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace RazorPad.UI.Editors
{
    public class JsonCodeEditor : CodeEditor
    {
        public JsonCodeEditor()
        {
            Editor.TextArea.IndentationStrategy = new JsonIndentationStrategy();
            InitializeFolding(new BraceFoldingStrategy());
            Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        }
    }

    public class JsonIndentationStrategy : CSharpIndentationStrategy
    {
        
    }
}
