using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;

namespace RazorPad.UI.CodeEditors
{
    public class XmlCodeEditor : CodeEditor
    {
        public XmlCodeEditor()
        {
            TextArea.IndentationStrategy = new DefaultIndentationStrategy();
            InitializeFolding(new XmlFoldingStrategy());
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
        }
    }
}