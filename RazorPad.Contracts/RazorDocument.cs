using System.Collections.Generic;

namespace RazorPad
{
    public class RazorDocument
    {
        public string Filename { get; set; }

        public IDictionary<string, string> Metadata { get; private set; }

        public IModelProvider ModelProvider { get; set; }

        public string Template { get; set; }

        public string TemplateBaseClassName { get; set; }

        public RazorDocumentKind DocumentKind
        {
            get
            {
                if(_documentKind != null)
                    return _documentKind.Value;

                if(string.IsNullOrWhiteSpace(Filename) || Filename.EndsWith(".cshtml"))
                    return RazorDocumentKind.TemplateOnly;

                return RazorDocumentKind.Full;
            }
            set { _documentKind = value; }
        }
        private RazorDocumentKind? _documentKind;

        public RazorDocument()
            : this(null)
        {
        }

        public RazorDocument(string template, IModelProvider modelProvider = null, IDictionary<string, string> metadata = null)
        {
            Metadata = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>());
            ModelProvider = modelProvider;
            Template = template ?? string.Empty;
            TemplateBaseClassName = "RazorPad.Compilation.TemplateBase";
        }

        public dynamic GetModel()
        {
            if (ModelProvider == null)
                return null;

            return ModelProvider.GetModel();
        }
    }
}
