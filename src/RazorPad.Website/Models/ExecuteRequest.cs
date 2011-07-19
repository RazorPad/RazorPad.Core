using System.Collections.Generic;

namespace RazorPad.Website.Models
{
    public class ExecuteRequest : ParseRequest
    {
        public IEnumerable<TemplateParameter> Parameters { get; set; }
    }

    public class TemplateParameter
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
    }
}