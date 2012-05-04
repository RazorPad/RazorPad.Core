using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;

namespace RazorPad.UI.CodeEditors
{
    public class JavaScriptCodeEditor : CodeEditor
    {
        public JavaScriptCodeEditor()
        {
            TextArea.IndentationStrategy = new IndentationStrategy();
            InitializeFolding(new FoldingStrategy());
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        }


        class IndentationStrategy : DefaultIndentationStrategy
        {
        }

        class FoldingStrategy : BraceFoldingStrategy
        {

        }
    }
}