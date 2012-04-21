using System.IO;
using System.Text;

namespace RazorPad.Persistence
{
    public class RazorDocumentLoader : IRazorDocumentLoader
    {
        private readonly XmlRazorDocumentLoader _xmlLoader;

        public Encoding Encoding { get; set; }

        public RazorDocumentLoader(Encoding encoding = null, XmlRazorDocumentLoader xmlLoader = null)
        {
            _xmlLoader = xmlLoader ?? new XmlRazorDocumentLoader();
            Encoding = encoding ?? Encoding.UTF8;
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
                return _xmlLoader.Load(stream);
            }

            return new RazorDocument(new StreamReader(stream, Encoding).ReadToEnd());
        }
    }
}