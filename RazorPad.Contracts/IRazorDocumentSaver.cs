using System;
using System.Diagnostics;
using System.IO;

namespace RazorPad.Persistence
{
    public interface IRazorDocumentSaver
    {
        void Save(RazorDocument document, Stream stream);
    }

    public static class RazorDocumentSaverExtensions
    {
        public static void Save(this IRazorDocumentSaver saver, RazorDocument document, string filename = null)
        {
            var destination = filename ?? document.Filename;

            if (string.IsNullOrWhiteSpace(destination))
                throw new ApplicationException("No filename specified!");

            document.DocumentKind = RazorDocument.GetDocumentKind(filename);

            using (var stream = File.Open(destination, FileMode.Truncate, FileAccess.Write))
            {
                saver.Save(document, stream);

                try { stream.Flush(); }
                catch(Exception ex)
                {
                    Trace.WriteLine("Error flushing file stream: " + ex);
                }
            }
        }
    }

}