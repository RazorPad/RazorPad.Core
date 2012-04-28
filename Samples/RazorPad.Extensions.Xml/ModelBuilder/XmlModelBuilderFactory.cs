using System.ComponentModel.Composition;
using RazorPad.Extensions.Xml.ModelProvider;
using RazorPad.Extensions.Xml.XmlBuilder;
using RazorPad.UI;

namespace RazorPad.Extensions.Xml.ModelBuilder
{
    [Export(typeof(IModelBuilderFactory))]
    public class XmlModelBuilderFactory : ModelBuilderFactory<XmlModelProvider, XmlModelBuilder>
    {
        protected override dynamic ViewModel(XmlModelProvider provider)
        {
            return new XmlModelBuilderViewModel(provider);
        }
    }
}