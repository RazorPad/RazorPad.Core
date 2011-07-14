using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using RazorPad.Compilation;
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
        public ActionResult Parse([Bind(Prefix="")]ParseRequest request)
        {
            return SafeExecute(compiler => {
                ParseResult result = new ParseResult();
                var writer = new StringWriter();
                var generatorResults = compiler.GenerateCode(request.Template, writer);
                result.SetGeneratorResults(generatorResults);
                result.GeneratedCode = writer.ToString();
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        public ActionResult Execute([Bind(Prefix="")]ExecuteRequest request)
        {
            return SafeExecute(compiler => {
                ExecuteResult result = new ExecuteResult();

                var writer = new StringWriter();
                var generatorResults = compiler.GenerateCode(request.Template, writer);

                result.SetGeneratorResults(generatorResults);
                result.GeneratedCode = writer.ToString();

                if (generatorResults.Success)
                {
                    CompilerResults compilerResults = compiler.Compile(generatorResults);

                    result.SetCompilerResults(compilerResults);

                    if(!compilerResults.Errors.HasErrors)
                    {
                        result.TemplateOutput = compiler.Execute(request.Template, request.Parameters);
                    }

                    result.Success = true;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        protected ActionResult SafeExecute(Func<ITemplateCompiler, ActionResult> action)
        {
            // TODO: Saftey-ize this to keep the baddies from doing baddie stuff
            return action(new TemplateCompiler());
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString()};
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new [] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

    }
}
