using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RazorPad
{
    public class RazorDocument
    {
        public const string DefaultTemplateBaseClassName = "RazorPad.Compilation.TemplateBase";

        public static readonly IEnumerable<string> SimpleTemplateExtensions =
            new[] { ".txt", ".cshtml", ".vbhtml" };

        public static volatile Func<string, RazorDocumentKind> GetDocumentKind = 
            GetDocumentKindInternal;

        public string Filename { get; set; }

        public IDictionary<string, string> Metadata { get; private set; }

        public IModelProvider ModelProvider { get; set; }

        public string Template { get; set; }

        public string TemplateBaseClassName { get; set; }

        public IEnumerable<string> References { get; set; }

        public RazorDocumentKind DocumentKind
        {
            get { return _documentKind.GetValueOrDefault(GetDocumentKind(Filename)); }
            set { _documentKind = value; }
        }
        private RazorDocumentKind? _documentKind;

        public RazorDocument()
            : this(null)
        {
        }

        public RazorDocument(string template, 
                IEnumerable<string> references = null, 
                IModelProvider modelProvider = null, 
                IDictionary<string, string> metadata = null
            )
        {
            Metadata = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>());
            ModelProvider = modelProvider;
            References = references ?? Enumerable.Empty<string>();
            Template = template ?? string.Empty;
            TemplateBaseClassName = DefaultTemplateBaseClassName;
        }

        public dynamic GetModel()
        {
            if (ModelProvider == null)
                return null;

            return ModelProvider.GetModel();
        }

        private static RazorDocumentKind GetDocumentKindInternal(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                var extension = Path.GetExtension(filename);
                if (SimpleTemplateExtensions.Contains(extension))
                    return RazorDocumentKind.TemplateOnly;
            }

            return RazorDocumentKind.Full;
        }
    }
}
