using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.UI
{
    public class RazorSyntaxTreeVisualizer
    {
        public string Visualize(SyntaxTreeNode node)
        {
            return ParseNode(node);
        }

        private string ParseNode(SyntaxTreeNode node, int tabLevel = -1)
        {
            var block = node as Block;

            var tabs = new string('\t', tabLevel + 1);

            if (block == null)
                return string.Format("{0}<{1}>", tabs, node);

            string children = string.Join("\r\n", block.Children.Select(x => ParseNode(x, tabLevel + 1)));

            return string.Format("{0}<{1}>\r\n{2}\r\n{0}</{1}>", tabs, block, children);
        }
    }
}
