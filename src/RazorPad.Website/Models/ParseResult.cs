using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.Website.Models
{
    [Serializable]
    public class ParseResult
    {
        public string ParsedDocument { get; set; }

        public string GeneratedCode { get; set; }

        public IEnumerable<TemplateMessage> Messages { get; set; }

        public bool Success { get; set; }


        public ParseResult()
        {
            Success = false;
        }

        public void SetGeneratorResults(GeneratorResults generatorResults)
        {
            if (generatorResults == null)
                return;

            Success = generatorResults.Success;

            ParsedDocument = ParseNode(generatorResults.Document);

            Messages =
                from error in generatorResults.ParserErrors
                select new TemplateMessage
                           {
                               Kind = TemplateMessageKind.Error,
                               Text = error.ToString(),
                           };
        }

        private string ParseNode(SyntaxTreeNode node, int tabLevel = -1)
        {
            var block = node as Block;

            string tabs = new string('\t', tabLevel + 1);

            if (block == null)
                return string.Format("{0}<{1}>", tabs, node);

            string children = string.Join("\r\n", block.Children.Select(x => ParseNode(x, tabLevel + 1)));

            return string.Format("{0}<{1}>\r\n{2}\r\n{0}</{1}>", tabs, block, children);
        }
    }
}