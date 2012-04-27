using System.ComponentModel.Composition;

namespace RazorPad.Extensions.Xml.ModelProvider
{
    [Export(typeof(IModelProviderFactory))]
    public class XmlModelProviderFactory : IModelProviderFactory
    {
        public IModelProvider Create(dynamic model = null)
        {
            return new XmlModelProvider(model: model);
        }
    }
}