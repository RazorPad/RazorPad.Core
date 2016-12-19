using System.IO;

namespace RazorPad.Persistence
{
    public interface IRazorDocumentLoader
    {
        RazorDocument Parse(string content);

        RazorDocument Load(string uri);
        RazorDocument Load(Stream stream);
    }
}