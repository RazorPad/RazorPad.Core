using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace RazorPad.Compilation
{
    public interface ITemplateExecutor
    {
        string Execute(string templateText, dynamic model, IEnumerable<string> assemblyReferences);
    }

    public static class TemplateExecutorExtensions
    {
        public static string Execute(this ITemplateExecutor executor, RazorDocument document)
        {
            Contract.Requires(document != null);
            return executor.Execute(document.Template, document.GetModel(), document.References);
        }
    }
}