using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RazorPad.Persistence
{
    [Export]
    public class XmlRazorDocumentSource : IRazorDocumentLoader, IRazorDocumentSaver
    {
        private readonly ModelProviders _modelProviderFactory;

        public Encoding Encoding { get; set; }

        [ImportingConstructor]
        public XmlRazorDocumentSource(ModelProviders modelProviderFactory = null)
        {
            _modelProviderFactory = modelProviderFactory ?? ModelProviders.Current;
            Encoding = Encoding.UTF8;
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
            if(source == null || source.Root == null)
                return null;

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

        public void Save(RazorDocument document, string filename = null)
        {
            var destination = filename ?? document.Filename;
            using (var stream = File.OpenWrite(destination))
                Save(document, stream);
        }

        public void Save(RazorDocument document, Stream stream)
        {
            var writer = new XmlTextWriter(stream, Encoding) { Formatting = Formatting.Indented };

            writer.WriteStartElement("RazorDocument");

            writer.WriteStartElement("Metadata");
            foreach (var datum in document.Metadata)
            {
                writer.WriteStartElement(datum.Key);
                writer.WriteString(datum.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            var serializedModel = document.ModelProvider.Serialize();
            writer.WriteStartElement("Model");
            writer.WriteAttributeString("Provider", new ModelProviderName(document.ModelProvider));
            writer.WriteCData(serializedModel);
            writer.WriteEndElement();

            writer.WriteStartElement("Template");
            writer.WriteAttributeString("BaseClass", document.TemplateBaseClassName);
            writer.WriteCData(document.Template);
            writer.WriteEndElement();


            writer.WriteEndElement();

            writer.Flush();
        }
    }
}
