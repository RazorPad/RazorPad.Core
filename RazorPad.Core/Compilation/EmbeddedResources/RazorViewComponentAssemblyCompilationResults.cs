using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace RazorPad.Compilation.EmbeddedResources
{
    public class RazorViewComponentAssemblyCompilationResults
    {
        public IDictionary<string, IEnumerable<CodeCompileUnit>> CodeCompileUnits { get; set; }

        public IEnumerable<CompilerResults> CompilerResults { get; internal set; }

        public IDictionary<string, IEnumerable<string>> EmbeddedRazorViews { get; private set; }

        public string MergedOutputFileName { get; set; }

        public bool OverwroteOriginalAssembly { get; set; }


        public RazorViewComponentAssemblyCompilationResults()
        {
            CodeCompileUnits = new Dictionary<string, IEnumerable<CodeCompileUnit>>();
            CompilerResults = Enumerable.Empty<CompilerResults>();
            EmbeddedRazorViews = new Dictionary<string, IEnumerable<string>>();
        }
    }
}