using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace RazorPad.Website.Models
{
    [Serializable]
    public class ExecuteResult : ParseResult
    {
        public string TemplateOutput { get; set; }

        public void SetCompilerResults(CompilerResults results)
        {
            if (results == null)
                return;

            // It's successful if there aren't any errors.
            // Warnings don't count as a failure
            Success = !results.Errors.HasErrors;

            Messages =
                (Messages ?? Enumerable.Empty<TemplateMessage>())
                .Union(
                    from error in results.Errors.Cast<CompilerError>()
                    select new TemplateMessage {
                        Kind = (error.IsWarning) ? TemplateMessageKind.Warning : TemplateMessageKind.Error,
                        Text = string.Format("Line {0} (col {1}): {2}", error.Line, error.Column, error.ErrorText),
                    }
                );
        }
    }
}