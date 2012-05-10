using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;

namespace RazorPad.UI.Editors
{
	public class RazorCodeEditor : CodeEditor
	{
		public RazorCodeEditor()
		{
            Editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			InitializeFolding(new XmlFoldingStrategy());
		    Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
		}

	}
}
