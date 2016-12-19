using System.ComponentModel.Composition;

namespace RazorPad.UI.ModelBuilders.CSharp
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