using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using RazorPad.Compilation;
using RazorPad.Framework;
using RazorPad.Website.Models;

namespace RazorPad.Website.Controllers
{
    [ValidateInput(false)]
    public class RazorPadController : Controller
    {
        public ActionResult Index()
        {
            return View("MainUI");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Parse([Bind(Prefix = "")]ParseRequest request)
        {
            ParseResult result = new ParseResult();
            var writer = new StringWriter();
            var generatorResults = new TemplateCompiler().GenerateCode(request.Template, writer);
            result.SetGeneratorResults(generatorResults);
            result.GeneratedCode = writer.ToString();

            dynamic testObject = new { Name = "Test", Number = 1234d };
            var json = JsonConvert.SerializeObject(testObject, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Execute([Bind(Prefix = "")]ExecuteRequest request)
        {
            ExecuteResult result = new ExecuteResult();

            var compiler = new TemplateCompiler();
            var writer = new StringWriter();

            var generatorResults = compiler.GenerateCode(request.Template, writer);
            result.SetGeneratorResults(generatorResults);
            result.GeneratedCode = writer.ToString();

            if (generatorResults.Success)
            {
                CompilerResults compilerResults = compiler.Compile(generatorResults);

                result.SetCompilerResults(compilerResults);

                if (!compilerResults.Errors.HasErrors)
                {
                    dynamic model;
                    
                    if(!string.IsNullOrEmpty(request.Model))
                        model = JsonConvert.DeserializeObject(request.Model, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto});
                    else
                        model = new DynamicDictionary();   

                    result.TemplateOutput = Sandbox.Execute(request.Template, model);
                }

                result.Success = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }


        class Sandbox : MarshalByRefObject
        {
            public static string Execute(string template, dynamic model = null)
            {
                // TODO: Run this in a sandbox
                var compiler = new TemplateCompiler();
                return compiler.Execute(template, model);
            }
        }
    }
}
