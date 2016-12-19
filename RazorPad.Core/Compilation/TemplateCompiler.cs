using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Razor;
using NLog;
using RazorPad.Compilation.Hosts;
using RazorPad.Framework;

namespace RazorPad.Compilation
{
    public class TemplateCompiler : ITemplateCompiler
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

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
            Log.Info("Compiling generated code...");

            var parameters = CompilationParameters.CompilerParameters;
            var codeProvider = CompilationParameters.CodeProvider;
            var generatedCode = generatorResults.GeneratedCode;

            Log.Debug("CodeProvider.CompileAssemblyFromDom()...");
            var compiledCode = codeProvider.CompileAssemblyFromDom(parameters, generatedCode);

            if (compiledCode.Errors.HasErrors)
            {
                Log.Warn("Compilation FAILED: {0}", compiledCode.Errors.Render());
            }
            else
            {
                Log.Info("Compilation succeeded");
            }

            return compiledCode;
        }

        public string Execute(string templateText, dynamic model, IEnumerable<string> assemblyReferences)
        {
            Log.Info("Executing template...");

            Log.Debug(() => string.Format("Template text: {0}", templateText));

            if (assemblyReferences != null)
            {
                Log.Debug(() => string.Format("Assembly References: {0}", string.Join(", ", assemblyReferences)));
                CompilationParameters.SetReferencedAssemblies(assemblyReferences);
            }

            dynamic instance = GetTemplateInstance(templateText);

            instance.Model = model ?? new DynamicDictionary();

            Log.Info("Executing...");
            instance.Execute();

            string templateOutput = instance.Buffer.ToString();

            Log.Info("Template executed.");
            Log.Debug(() => string.Format("Executed template output: {0}", templateOutput));            

            return templateOutput;
        }

        private dynamic GetTemplateInstance(string templateText, RazorEngineHost host = null)
        {
            Log.Info("Getting template instance...");

            host = host ?? RazorEngineHostFactory.Invoke(CompilationParameters.Language);

            var generatorResults = GenerateCode(templateText, null, host: host);

            if (!generatorResults.Success)
            {
                Log.Error("Failed to parse template: {0}", generatorResults.ParserErrors.Render());
                throw new CodeGenerationException(generatorResults);
            }

            var compilerResults = Compile(generatorResults);

            if (compilerResults.Errors.Count > 0)
            {
                Log.Error("Compilation failed with {0} errors", compilerResults.Errors.Count);
                throw new CompilationException(compilerResults);
            }

            var typeName = string.Format("{0}.{1}", host.DefaultNamespace, host.DefaultClassName);
            return CreateTemplateInstance(typeName, compilerResults);
        }

        private dynamic CreateTemplateInstance(string typeName, CompilerResults compilerResults)
        {
            Log.Info("Creating instance of template {0}...", typeName);

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
            Log.Info("Generating code...");

            host = host ?? RazorEngineHostFactory(CompilationParameters.Language);
            var engine = new RazorTemplateEngine(host);

            var results = engine.GenerateCode(templateText.ToTextReader());

            if (codeWriter == null)
            {
                Log.Debug("No code writer provided -- skipping primary language code generation");
            }
            else
            {
                var codeProvider = CompilationParameters.CodeProvider;
                var generatedCode = results.GeneratedCode;

                Log.Debug("CodeProvider.GenerateCodeFromCompileUnit()...");
                codeProvider.GenerateCodeFromCompileUnit(generatedCode, codeWriter, CodeGeneratorOptions);
            }

            return results;
        }
    }
}