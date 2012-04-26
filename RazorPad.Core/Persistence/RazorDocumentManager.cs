using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;

namespace RazorPad.Persistence
{
    [Export]
    public class RazorDocumentManager : IRazorDocumentLoader, IRazorDocumentSaver
    {
        private readonly XmlRazorDocumentSource _xmlDataSource;

        public Encoding Encoding
        {
            get { return _xmlDataSource.Encoding; }
            set { _xmlDataSource.Encoding = value; }
        }

        [ImportingConstructor]
        public RazorDocumentManager()
            : this(null, null)
        {
        }

        public RazorDocumentManager(Encoding encoding, XmlRazorDocumentSource xmlLoader)
        {
            _xmlDataSource = xmlLoader ?? new XmlRazorDocumentSource();
            Encoding = encoding ?? Encoding;
        }

        public RazorDocument Parse(string content)
        {
            using (var stream = new MemoryStream(Encoding.GetBytes(content)))
                return Load(stream);
        }

        public RazorDocument Load(string uri)
        {
            using (var stream = File.OpenRead(uri))
            {
                var document = Load(stream);
                document.Filename = uri;
                return document;
            }
        }

        public RazorDocument Load(Stream stream)
        {
            var reader = new StreamReader(stream, Encoding);
            var firstLine = reader.ReadLine() ?? string.Empty;
            
            stream.Seek(0, SeekOrigin.Begin);

            if(firstLine.Contains("<? xml") || firstLine.Contains("<RazorDocument>"))
            {
                return _xmlDataSource.Load(stream);
            }

            return new RazorDocument(new StreamReader(stream, Encoding).ReadToEnd());
        }

        public void Save(RazorDocument document, string fileName = null)
        {
            var destination = fileName ?? document.Filename;

            if (string.IsNullOrWhiteSpace(destination))
                throw new ApplicationException("No filename specified!");

            Save(document, File.Open(destination, FileMode.OpenOrCreate, FileAccess.Write));
        }

        public void Save(RazorDocument document, Stream stream)
        {
            if (document.DocumentKind == RazorDocumentKind.TemplateOnly)
            {
                SavePlainRazorTemplate(document, stream);
                return;
            }

            _xmlDataSource.Save(document, stream);
        }

        private static void SavePlainRazorTemplate(RazorDocument document, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
                writer.Write(document.Template);
        }
    }
}