using System.IO;

namespace RazorPad.Persistence
{
    public interface IRazorDocumentLoader
    {
        RazorDocument Load(string content);
        RazorDocument Load(Stream stream);
    }
}