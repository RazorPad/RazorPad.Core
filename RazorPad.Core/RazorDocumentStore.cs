using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RazorPad
{
    public class RazorDocumentStore
    {
        private readonly ModelProviderFactory _modelProviderFactory;

        public RazorDocumentStore(ModelProviderFactory modelProviderFactory)
        {
            _modelProviderFactory = modelProviderFactory ?? new ModelProviderFactory();
        }

        public RazorDocument Load(string uri)
        {
            var source = XDocument.Load(uri);
            
            var document = Load(source);
            document.Filename = uri;
            
            return document;
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

        public RazorDocument Parse(string documentContents)
        {
            var source = XDocument.Parse(documentContents);
            return Load(source);
        }
    }
}
