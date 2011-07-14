using System.Collections.Generic;

namespace RazorPad.Website.Models
{
    public class ParseRequest
    {
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; set; }

        public string Template { get; set; }
    }
}