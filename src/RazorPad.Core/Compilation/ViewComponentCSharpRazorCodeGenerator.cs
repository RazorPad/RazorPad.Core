using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Mvc.Razor;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;

namespace RazorPad.Compilation
{
    public class ViewComponentCSharpRazorCodeGenerator : MvcCSharpRazorCodeGenerator
    {
        [ImportMany(typeof(CSharpRazorCodeGeneratorSpanVisitor))]
        public IEnumerable<CSharpRazorCodeGeneratorSpanVisitor> Vistors { get; set; }


        public ViewComponentCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host) 
            : base(className, rootNamespaceName, sourceFileName, host)
        {
        }

        protected override bool TryVisitSpecialSpan(Span span)
        {
            var vistors = (Vistors ?? Enumerable.Empty<CSharpRazorCodeGeneratorSpanVisitor>());

            return base.TryVisitSpecialSpan(span)
                || vistors.FirstOrDefault(x => x.TryVisit(span)) != null;
        }
    }
}
