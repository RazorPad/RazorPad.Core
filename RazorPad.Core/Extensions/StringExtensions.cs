using System.IO;
using System.Text;

namespace RazorPad.Core
{
    public static class StringExtensions
    {
        public static TextReader ToTextReader(this string source)
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(source ?? string.Empty));
            return new StreamReader(stream);
        }
    }
}