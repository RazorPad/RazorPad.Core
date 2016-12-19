using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Razor;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using NLog;

namespace RazorPad.Compilation.EmbeddedResources
{
    public class RazorViewComponentAssembly
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static readonly Func<string, bool> ResourceIsRazorView = EmbeddedTemplateHost.IsRazorView;

        private readonly string _originalAssemblyFileName;
        private RazorViewComponentAssemblyCompilationResults _compilationResultMessage;

        protected Assembly Assembly { get; private set; }
        
        public string WorkingFolder { get; private set; }



        public RazorViewComponentAssembly(string assemblyFileName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(assemblyFileName));

            _originalAssemblyFileName = assemblyFileName;
        }


        public RazorViewComponentAssemblyCompilationResults CompileEmbeddedViews()
        {
            _compilationResultMessage = new RazorViewComponentAssemblyCompilationResults();

            Initialize();

            IEnumerable<string> embeddedRazorViewResourceNames = 
                Assembly.GetManifestResourceNames().Where(ResourceIsRazorView).ToArray();

            if (!embeddedRazorViewResourceNames.Any())
            {
                Log.Debug("No embedded Razor views were found in this assembly.");
                return _compilationResultMessage;
            }

            Log.Debug(() => string.Format("Found the following embedded Razor views: {0}", 
                                          string.Join("\r\n", embeddedRazorViewResourceNames)));

            IEnumerable<IGrouping<RazorCodeLanguage, string>> resourcesGroupedByLanguage =
                from resourceName in embeddedRazorViewResourceNames
                let resourceInfo = new FileInfo(resourceName)
                let language = RazorCodeLanguage.GetLanguageByExtension(resourceInfo.Extension)
                group resourceName by language into groupedResources
                select groupedResources;

            var compiledAssemblies = CompileResourcesByLanguage(resourcesGroupedByLanguage);

            bool everythingCompiledSuccessfully = compiledAssemblies.All(x => !x.Errors.HasErrors);

            if (!everythingCompiledSuccessfully)
            {
                throw new RazorViewComponentAssemblyCompilationException(compiledAssemblies);
            }

            Log.Debug("Done!");

            return _compilationResultMessage;
        }

        private void Initialize()
        {
            Log.Debug("Initializing Razor View Component Assembly {0}...", _originalAssemblyFileName);

            WorkingFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(WorkingFolder);
            Log.Debug("Working folder: {0}", WorkingFolder);

            var workingAssemblyFileName = Path.Combine(WorkingFolder, Path.GetFileName(_originalAssemblyFileName));

            Log.Debug("Copying original assembly {0} to working file {1}...", _originalAssemblyFileName, workingAssemblyFileName);
            File.Copy(_originalAssemblyFileName, workingAssemblyFileName, true);

            Log.Debug("Loading assembly: {0}", workingAssemblyFileName);
            Assembly = Assembly.LoadFrom(workingAssemblyFileName);
        }

        private IEnumerable<CompilerResults> CompileResourcesByLanguage(IEnumerable<IGrouping<RazorCodeLanguage, string>> resourcesGroupedByLanguage)
        {
            var compilerResults = new List<CompilerResults>();

            foreach (var resourceNameGroup in resourcesGroupedByLanguage.ToArray())
            {
                RazorCodeLanguage language = resourceNameGroup.Key;
                string languageDisplayName = language.GetType().Name;
                IEnumerable<string> resourceNames = resourceNameGroup;

                Log.Debug("Compiling {0} {1} resources", resourceNames.Count(), languageDisplayName);

                _compilationResultMessage.EmbeddedRazorViews.Add(languageDisplayName, resourceNames);

                var codeCompileUnits = GenerateCode(resourceNames).ToArray();

                _compilationResultMessage.CodeCompileUnits.Add(languageDisplayName, codeCompileUnits);

                var compilerResult = CompileGeneratedCode(language, codeCompileUnits);

                compilerResults.Add(compilerResult);
            }

            _compilationResultMessage.CompilerResults = compilerResults;

            return compilerResults;
        }

        private IEnumerable<CodeCompileUnit> GenerateCode(IEnumerable<string> resourceNames)
        {
            Log.Debug(() => string.Format("Generating code for resources: {0}", 
                                          string.Join("; ", resourceNames.ToArray())));

            var codeCompileUnits = new List<CodeCompileUnit>();

            foreach (var resourceName in resourceNames)
            {
                var host = new EmbeddedTemplateHost(Assembly, resourceName);
                var engine = new RazorTemplateEngine(host);
                var generatorResults = engine.GenerateCode(host.GetEmbeddedResource());

                codeCompileUnits.Add(generatorResults.GeneratedCode);
            }

            return codeCompileUnits;
        }

        private CompilerResults CompileGeneratedCode(RazorCodeLanguage language, IEnumerable<CodeCompileUnit> codeCompileUnits)
        {
            Log.Debug("Compiling generated code for {0}", language.GetType().Name);

            CodeDomProvider codeProvider = GetCodeProvider(language);

            var compilerParameters = new CompilerParameters();

            if (compilerParameters.GenerateInMemory)
            {
                Log.Debug("Generating assemblies in memory");
            }
            else
            {
                Log.Debug("Output assembly: {0}", compilerParameters.OutputAssembly);
            }

            var compilerResults =
                codeProvider.CompileAssemblyFromDom(compilerParameters, codeCompileUnits.ToArray());

            return compilerResults;
        }

        protected virtual CodeDomProvider GetCodeProvider(RazorCodeLanguage language)
        {
            if (language is VBRazorCodeLanguage)
                return new VBCodeProvider();
            else
                return new CSharpCodeProvider();
        }
    }
}
