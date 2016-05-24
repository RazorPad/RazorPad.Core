using RazorPad.Extensions.Xml.ModelProvider;
using RazorPad.UI.ModelBuilders;

namespace RazorPad.Extensions.Xml.ModelBuilder
{
    public class XmlModelBuilderViewModel : ModelBuilderViewModel<XmlModelProvider>
    {
        public string Xml
        {
            get { return ModelProvider.Xml; }
            set { ModelProvider.Xml = value; }
        }

        public XmlModelBuilderViewModel(XmlModelProvider modelProvider = null) 
            : base(modelProvider ?? new XmlModelProvider())
        {
        }

        protected override void OnModelChanged()
        {
            OnPropertyChanged("Xml");
        }
    }
}
