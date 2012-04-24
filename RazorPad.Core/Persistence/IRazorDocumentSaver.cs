using System.IO;

namespace RazorPad.Persistence
{
    public interface IRazorDocumentSaver
    {
        void Save(RazorDocument document, string fileName = null);
        void Save(RazorDocument document, Stream stream);
    }
}