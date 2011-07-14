using System.Collections.Generic;
using System.Linq;
using System.Web.Razor;

namespace RazorPad.Website.Models
{
    public class ParseResult
    {
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
            Messages =
                from error in generatorResults.ParserErrors
                select new TemplateMessage
                           {
                               Kind = TemplateMessageKind.Error,
                               Text = error.ToString(),
                           };
        }
    }
}