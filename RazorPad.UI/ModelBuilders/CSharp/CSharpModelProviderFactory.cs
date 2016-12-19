using System.ComponentModel.Composition;
using RazorPad.Providers;

namespace RazorPad.UI.ModelBuilders.CSharp
{
    [Export(typeof(IModelProviderFactory))]
    public class CSharpModelProviderFactory : ModelProviderFactory<CSharpModelProvider>
    {
    }
}