using System.Collections.Generic;
using RazorPad.Compilation;
using RazorPad.Providers;

namespace RazorPad
{
    public class RazorDocument
    {
        public string Filename { get; set; }

        public IDictionary<string, string> Metadata { get; private set; }

        public IModelProvider ModelProvider { get; set; }

        public string Template { get; set; }

        public string TemplateBaseClassName { get; set; }


        public RazorDocument(string template, object model, IDictionary<string, string> metadata = null)
            : this(template, new BasicModelProvider(model: model), metadata)
        {
        }

        public RazorDocument(string template = null, IModelProvider modelProvider = null, IDictionary<string, string> metadata = null)
        {
            Metadata = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>());
            ModelProvider = modelProvider ?? new BasicModelProvider();
            Template = template ?? string.Empty;
            TemplateBaseClassName = typeof(TemplateBase).FullName;
        }


        public dynamic GetModel()
        {
            if (ModelProvider == null)
                return null;

            return ModelProvider.GetModel();
        }
    }
}
