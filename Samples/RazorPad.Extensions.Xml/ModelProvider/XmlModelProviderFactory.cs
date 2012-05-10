using System.ComponentModel.Composition;
using RazorPad.Providers;

namespace RazorPad.Extensions.Xml.ModelProvider
{
    [Export(typeof(IModelProviderFactory))]
    public class XmlModelProviderFactory : ModelProviderFactory<XmlModelProvider>
    {
    }
}