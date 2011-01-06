using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Razor;

namespace RazorPad.Compilation.EmbeddedResources
{
    public class EmbeddedTemplateHost : RazorPadHost
    {
        private static readonly Regex ResourceNameRegex = new Regex(@"^(?<Namespace>.*)\.(?<ClassName>.*)\.(?<Language>cs|vb)html$");

        private readonly Assembly _assembly;
        private readonly string _resourceName;

        public EmbeddedTemplateHost(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            DefaultClassName = GetClassName(resourceName);
            DefaultNamespace = GetNamespace(resourceName);
            CodeLanguage = GetCodeLanguage(resourceName);
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }


        protected override void AddAssemblyReferences(CodeCompileUnit codeCompileUnit)
        {
            IEnumerable<string> referencedAssemblies =
                _assembly.GetReferencedAssemblies()
                    .Select(x => x.CodeBase)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Replace("file:///", string.Empty));

            codeCompileUnit.ReferencedAssemblies.Add(_assembly.Location);
            codeCompileUnit.ReferencedAssemblies.AddRange(referencedAssemblies.ToArray());
        }

        protected override void AddNamespaceImports(CodeNamespace generatedNamespace)
        {
            var nameSpace = GetNamespace(_resourceName);
            generatedNamespace.Imports.Add(new CodeNamespaceImport(nameSpace));
        }

        protected string GetClassName(string resourceName)
        {
            var match = ResourceNameRegex.Match(resourceName);
            var className = match.Groups["ClassName"].Value;
            return className;
        }

        protected string GetNamespace(string resourceName)
        {
            var match = ResourceNameRegex.Match(resourceName);
            var className = match.Groups["Namespace"].Value;
            return className;
        }

        protected RazorCodeLanguage GetCodeLanguage(string resourceName)
        {
            var extension = new FileInfo(resourceName).Extension;
            var language = RazorCodeLanguage.GetLanguageByExtension(extension);
            return language;
        }

        public TextReader GetEmbeddedResource()
        {
            var stream = _assembly.GetManifestResourceStream(_resourceName);

            // TODO: Null check

            return new StreamReader(stream);
        }


        public static bool IsRazorView(string resourceName)
        {
            return ResourceNameRegex.IsMatch(resourceName);
        }
    }
}
