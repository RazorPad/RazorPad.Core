using System.Web.Razor;
using RazorPad.Compilation;

namespace RazorPad.Website.Models
{
    public class ExecuteRequest : ParseRequest
    {
        public string Model { get; set; }

        public TemplateLanguage Language { get; set; }

        public RazorCodeLanguage RazorLanguage
        {
            get
            {
                switch (Language)
                {
                    case(TemplateLanguage.VisualBasic):
                        return new VBRazorCodeLanguage();
                    default:
                        return new CSharpRazorCodeLanguage();
                }
            }
            set { }
        }
    }
}