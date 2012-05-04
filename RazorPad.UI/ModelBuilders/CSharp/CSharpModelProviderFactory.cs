using System.ComponentModel.Composition;

namespace RazorPad.UI.ModelBuilders.CSharp
{
    [Export(typeof(IModelProviderFactory))]
    public class CSharpModelProviderFactory : IModelProviderFactory
    {
        public IModelProvider Create(dynamic model = null)
        {
            return new CSharpModelProvider(model);
        }
    }
}