using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RazorPad.Persistence
{
    public class XmlRazorDocumentLoader : IRazorDocumentLoader
    {
        private readonly ModelProviderFactory _modelProviderFactory;

        public XmlRazorDocumentLoader(ModelProviderFactory modelProviderFactory = null)
        {
            _modelProviderFactory = modelProviderFactory ?? new ModelProviderFactory();
        }

        public RazorDocument Parse(string document)
        {
            var source = XDocument.Parse(document);
            return Load(source);
        }

        public RazorDocument Load(string uri)
        {
            var source = XDocument.Load(uri);
            return Load(source);
        }

        public RazorDocument Load(Stream stream)
        {
            var source = XDocument.Load(stream);
            return Load(source);
        }

        public RazorDocument Load(XDocument source)
        {
            var root = source.Root;
            var metadataEl = root.Element("Metadata") ?? new XElement("Metadata");
            var modelEl = root.Element("Model") ?? new XElement("Model");
            var templateEl = root.Element("Template") ?? new XElement("Template");

            var modelProviderEl = modelEl.Attribute("Provider");
            var modelProviderName = (modelProviderEl == null) ? "Json" : modelProviderEl.Value;
            var modelProvider = _modelProviderFactory.Create(modelProviderName, modelEl.Value);

            IDictionary<string, string> metadata =
                metadataEl.Elements()
                    .Select(x => new KeyValuePair<string, string>(x.Name.LocalName, x.Value))
                    .ToDictionary(val => val.Key, val => val.Value);

            return new RazorDocument(templateEl.Value, modelProvider, metadata);
        }
    }
}
