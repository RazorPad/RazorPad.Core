using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;

namespace RazorPad.UI.CodeEditors
{
	public class RazorCodeEditor : CodeEditor
	{
		public RazorCodeEditor()
		{
            TextArea.IndentationStrategy = new IndentationStrategy();
            InitializeFolding(new FoldingStrategy());
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
		}

        
        class IndentationStrategy : DefaultIndentationStrategy
        {
        }

        class FoldingStrategy : XmlFoldingStrategy
        {
            
        }
	}
}
