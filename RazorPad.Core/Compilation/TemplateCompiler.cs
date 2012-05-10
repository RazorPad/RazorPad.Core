using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Razor;
using RazorPad.Compilation.Hosts;
using RazorPad.Framework;

namespace RazorPad.Compilation
{
    public class TemplateCompiler : ITemplateCompiler
    {
        public CodeGeneratorOptions CodeGeneratorOptions { get; private set; }

        public TemplateCompilationParameters CompilationParameters { get; set; }

        public Func<RazorCodeLanguage,RazorEngineHost> RazorEngineHostFactory
        {
            get { return _razorEngineHostFactory ?? (language => new RazorPadHost(language)); }
            set { _razorEngineHostFactory = value; }
        }
        private Func<RazorCodeLanguage, RazorEngineHost> _razorEngineHostFactory;

        public Func<Type,object> TemplateInstanceInstatiator
        {
            get { return _templateInstanceInstatiator ?? Activator.CreateInstance; }
            set { _templateInstanceInstatiator = value; }
        }
        private Func<Type, object> _templateInstanceInstatiator;


        public TemplateCompiler()
            : this(null)
        {
        }

        public TemplateCompiler(TemplateCompilationParameters templateCompilationParameters)
        {
            CompilationParameters = templateCompilationParameters ?? TemplateCompilationParameters.CSharp;
            CodeGeneratorOptions = new CodeGeneratorOptions
                                       {
                                           BlankLinesBetweenMembers = false,
                                           BracingStyle = "C"
                                       };
        }


        public void AddAssemblyReference(Type type)
        {
            CompilationParameters.AddAssemblyReference(type.Assembly.Location);
        }

        public void AddAssemblyReference(Assembly assembly)
        {
            CompilationParameters.AddAssemblyReference(assembly.Location);
        }

        public void AddAssemblyReference(string location)
        {
            CompilationParameters.AddAssemblyReference(location);
        }


        public CompilerResults Compile(GeneratorResults generatorResults)
        {
            Trace.TraceInformation("Compiling generated code...");

            var parameters = CompilationParameters.CompilerParameters;
            var codeProvider = CompilationParameters.CodeProvider;
            var generatedCode = generatorResults.GeneratedCode;

            var compiledCode = codeProvider.CompileAssemblyFromDom(parameters, generatedCode);

            if (compiledCode.Errors.HasErrors)
            {
                Trace.TraceWarning("Compilation FAILED");
                Trace.WriteLine(compiledCode.Errors.Render());
            }
            else
            {
                Trace.TraceInformation("Compilation succeeded");
            }

            return compiledCode;
        }

        public string Execute(RazorDocument document)
        {
            return Execute(document.Template, document.GetModel());
        }

        public string Execute(string templateText, dynamic model)
        {
            Trace.WriteLine(string.Format("Template text: {0}", templateText));

            dynamic instance = GetTemplateInstance(templateText);

            instance.Model = model ?? new DynamicDictionary();

            Trace.TraceInformation("Executing template...");
            instance.Execute();

            string templateOutput = instance.Buffer.ToString();

            Trace.WriteLine(string.Format("Executed template output: {0}", templateOutput));

            return templateOutput;
        }

        private dynamic GetTemplateInstance(string templateText, RazorEngineHost host = null)
        {
            host = host ?? RazorEngineHostFactory.Invoke(CompilationParameters.Language);

            var generatorResults = GenerateCode(templateText, null, host: host);

            if (!generatorResults.Success)
            {
                Trace.TraceError("Failed to parse template: ", generatorResults.ParserErrors.Render());
                throw new CodeGenerationException(generatorResults);
            }

            var compilerResults = Compile(generatorResults);

            if (compilerResults.Errors.Count > 0)
            {
                throw new CompilationException(compilerResults);
            }

            var typeName = string.Format("{0}.{1}", host.DefaultNamespace, host.DefaultClassName);
            return CreateTemplateInstance(typeName, compilerResults);
        }

        private dynamic CreateTemplateInstance(string typeName, CompilerResults compilerResults)
        {
            Trace.TraceInformation("Creating instance of template {0}...", typeName);

            var type = compilerResults.CompiledAssembly.GetType(typeName);
            return TemplateInstanceInstatiator(type);
        }


        public string GenerateCode(string templateText, RazorEngineHost host = null)
        {
            GeneratorResults results;
            return GenerateCode(templateText, out results, host);
        }

        public string GenerateCode(string templateText, out GeneratorResults results, RazorEngineHost host = null)
        {
            using (var writer = new StringWriter())
            {
                results = GenerateCode(templateText, writer, host);
                return writer.GetStringBuilder().ToString();
            }
        }

        public GeneratorResults GenerateCode(string templateText, TextWriter codeWriter, RazorEngineHost host = null)
        {
            host = host ?? RazorEngineHostFactory(CompilationParameters.Language);
            var engine = new RazorTemplateEngine(host);

            var results = engine.GenerateCode(templateText.ToTextReader());

            if (codeWriter != null)
            {
                var codeProvider = CompilationParameters.CodeProvider;
                var generatedCode = results.GeneratedCode;
                
                codeProvider.GenerateCodeFromCompileUnit(generatedCode, codeWriter, CodeGeneratorOptions);
            }

            return results;
        }
    }
}