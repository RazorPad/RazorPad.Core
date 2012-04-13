using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Razor;
using RazorPad.Compilation.Hosts;
using RazorPad.Core;

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


        public CompilerResults Compile(string templateText)
        {
            var generatorResults = GenerateCode(templateText);
            var compilerResults = Compile(generatorResults);

            return compilerResults;
        }

        public CompilerResults Compile(GeneratorResults generatorResults)
        {
            var parameters = CompilationParameters.CompilerParameters;
            var codeProvider = CompilationParameters.CodeProvider;
            var generatedCode = generatorResults.GeneratedCode;

            var compiledCode = codeProvider.CompileAssemblyFromDom(parameters, generatedCode);
            return compiledCode;
        }

        public string Execute(string templateText, dynamic model = null, RazorEngineHost host = null)
        {
            dynamic instance = GetTemplateInstance(templateText);

            if (model != null)
                instance.Model = model;

            instance.Execute();

            var templateOutput = instance.Buffer.ToString();

            return templateOutput;
        }

        private dynamic GetTemplateInstance(string templateText, RazorEngineHost host = null)
        {
            host = host ?? RazorEngineHostFactory.Invoke(CompilationParameters.Language);

            var generatorResults = GenerateCode(templateText, null, host: host);

            if (!generatorResults.Success)
                throw new CodeGenerationException(generatorResults);

            var compilerResults = Compile(generatorResults);

            if (compilerResults.Errors.Count > 0)
                throw new CompilationException(compilerResults);

            var typeName = string.Format("{0}.{1}", host.DefaultNamespace, host.DefaultClassName);
            var type = compilerResults.CompiledAssembly.GetType(typeName);
            return TemplateInstanceInstatiator.Invoke(type);
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
            host = host ?? RazorEngineHostFactory.Invoke(CompilationParameters.Language);
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

        public Type GetTemplateModelType(string templateText)
        {
            TemplateBase instance = GetTemplateInstance(templateText);

            if (instance == null)
                return null;

            if (instance.Model != null)
                return instance.Model.GetType();

            var type = instance.GetType()
                        .GetProperties()
                        .Where(x => x.Name == "Model")
                        .Select(x => x.PropertyType)
                        .FirstOrDefault();

            return type;
        }
    }
}