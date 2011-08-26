using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using Microsoft.CSharp;
using RazorPad.Compilation;
using RazorPad.Compilation.Hosts;
using RazorPad.Website.Models;
using System.Text.RegularExpressions;

namespace RazorPad.Website.Controllers
{
    [ValidateInput(false)]
    public class RazorPadController : Controller
    {
        public ActionResult Index()
        {
            return View("MainUI");
        }


        public ActionResult beta()
        {
            return View("Ide");
        }


        public ActionResult Parse([Bind(Prefix = "")]ParseRequest request)
        {
            ParseResult result = new ParseResult();
            var writer = new StringWriter();
            var generatorResults = new TemplateCompiler().GenerateCode(request.Template, writer);
            result.SetGeneratorResults(generatorResults);
            result.GeneratedCode = writer.ToString();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Execute([Bind(Prefix = "")]ExecuteRequest request)
        {
            ExecuteResult result = new ExecuteResult();

            var templateParams = TemplateCompilationParameters.CreateFromLanguage(request.Language);
            var compiler = new TemplateCompiler(templateParams);

            var writer = new StringWriter();

            var templ = TransformRequest(request.Template);
            var generatorResults = compiler.GenerateCode(templ, writer, new RazorPadMvcEngineHost(request.RazorLanguage));
            result.SetGeneratorResults(generatorResults);
            result.GeneratedCode = ExtractCode(writer);

            if (generatorResults.Success)
            {
                CompilerResults compilerResults = compiler.Compile(generatorResults);

                result.SetCompilerResults(compilerResults);

                if (!compilerResults.Errors.HasErrors)
                {
                    //dynamic model;

                    //if (!string.IsNullOrEmpty(request.Model))
                    //    model = JsonConvert.DeserializeObject(request.Model, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                    //else
                    //    model = new DynamicDictionary();

                    
                    result.TemplateOutput = Sandbox.Execute(request.Language, templ, request.Model);
                }

                result.Success = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string TransformRequest(string p)
        {

            var transformedTemplate = "@using System.Linq;@model System.Collections.Generic.IEnumerable<RazorPad.DynamicModel.JsonProps>\n" + Regex.Replace(p, "@Model\\.([a-zA-Z0-9]+)", "@Model.First(x => x.Name == \"$1\").Value");
            return transformedTemplate;
        }

        private static dynamic GetCompiledModel(string model)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" })
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = true
            };

            var results = provider.CompileAssemblyFromSource(parameters, model);
            return results.CompiledAssembly.CreateInstance("Dummy");
            //results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }

        /// <summary>
        /// Takes a writer with a bunch of code and extracts only the code that we're interested in
        /// (excluding boilerplate generated code comments, empty classes, etc.)
        /// </summary>
        private string ExtractCode(StringWriter writer)
        {
            // TODO: Extract the right stuff
            return writer.ToString();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

        class Sandbox : MarshalByRefObject
        {
            public static string Execute(TemplateLanguage language, string template, dynamic model = null)
            {
                var templateParams = TemplateCompilationParameters.CreateFromLanguage(language);

                var compiler = new TemplateCompiler(templateParams);

                // TODO: Run this in a sandbox
                return compiler.Execute(template, model, new RazorPadMvcEngineHost(templateParams.Language));
            }
        }
    }
}
