using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;

namespace RazorPad.Compilation.Hosts
{
    public class RazorPadHost : RazorEngineHost
    {
        private static readonly ISet<string> GlobalImports = new HashSet<string>();

        public RazorPadHost(RazorCodeLanguage language = null)
        {
            
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            DefaultBaseClass = typeof(TemplateBase).FullName;
            DefaultClassName = "CompiledTemplate";
            DefaultNamespace = "RazorPad.Runtime";
            CodeLanguage = language;
            GeneratedClassContext = new GeneratedClassContext(
                GeneratedClassContext.DefaultExecuteMethodName, 
                GeneratedClassContext.DefaultWriteMethodName,
                GeneratedClassContext.DefaultWriteLiteralMethodName,
                "WriteTo", "WriteLiteralTo", 
                typeof(HelperResult).FullName);
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public override MarkupParser CreateMarkupParser()
        {
            return new HtmlMarkupParser();
        }

        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator)
            {
                return new ViewComponentCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);
            }
            if (incomingCodeGenerator is VBRazorCodeGenerator)
            {
                return new VBRazorCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);
            }

            return base.DecorateCodeGenerator(incomingCodeGenerator);
        }

        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser)
            {
                return new RazorPadCSharpRazorCodeParser();
            }

            return base.DecorateCodeParser(incomingCodeParser);
        }

        public override void PostProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            base.PostProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);

            AddAssemblyReferences(codeCompileUnit);
            AddNamespaceImports(generatedNamespace);
        }


        protected virtual void AddAssemblyReferences(CodeCompileUnit codeCompileUnit)
        {
            IEnumerable<Assembly> defaultIncludes = new[] {
                typeof(TemplateBase).Assembly,
                typeof(DynamicAttribute).Assembly,
                typeof(INotifyPropertyChanged).Assembly,
                typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly,
            };

            IEnumerable<string> referencedAssemblies =
                defaultIncludes.Select(x => x.Location);

            codeCompileUnit.ReferencedAssemblies.AddRange(referencedAssemblies.ToArray());
        }

        protected virtual void AddNamespaceImports(CodeNamespace generatedNamespace)
        {
            var namespaces = new List<string> {"System"};

            // Import the globally-defined imports
            namespaces.AddRange(GetGlobalImports());

            var namespaceImports = namespaces.Select(s => new CodeNamespaceImport(s));
            generatedNamespace.Imports.AddRange(namespaceImports.ToArray());
        }

        public static IEnumerable<string> GetGlobalImports()
        {
            return GlobalImports.ToArray();
        }

        public static void AddGlobalImport(string importedNamespace)
        {
            GlobalImports.Add(importedNamespace);
        }
    }
}
