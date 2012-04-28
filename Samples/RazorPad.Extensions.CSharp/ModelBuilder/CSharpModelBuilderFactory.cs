using System.ComponentModel.Composition;
using RazorPad.Extensions.CSharp.ModelProvider;
using RazorPad.Extensions.CSharp.CSharpBuilder;
using RazorPad.UI;

namespace RazorPad.Extensions.CSharp.ModelBuilder
{
    [Export(typeof(IModelBuilderFactory))]
    public class CSharpModelBuilderFactory : ModelBuilderFactory<CSharpModelProvider, CSharpModelBuilder>
    {
        protected override dynamic ViewModel(CSharpModelProvider provider)
        {
            return new CSharpModelBuilderViewModel(provider);
        }
    }
}