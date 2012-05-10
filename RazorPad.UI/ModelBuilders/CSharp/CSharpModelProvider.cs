using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.CSharp;
using RazorPad.Extensions;

namespace RazorPad.UI.ModelBuilders.CSharp
{
    public class CSharpModelProvider : RazorPad.ModelProvider
    {
        private static readonly Lazy<string> ClassTemplate = new Lazy<string>(LoadClassTemplate);

        public string Code
        {
            get { return _code; }
            set
            {
                if (_code == value)
                    return;

                _code = value;
                TriggerModelChanged();
            }
        }
        private string _code;


        public CSharpModelProvider()
        {
            Code = ClassTemplate.Value;
        }


        public override string Serialize()
        {
            return Code;
        }

        public override void Deserialize(string serialized)
        {
            Code = serialized;
        }

        protected override dynamic RebuildModel()
        {
            var results = new CSharpCodeProvider().CompileAssemblyFromSource(
                new CompilerParameters(new[] {
                    typeof(System.String).Assembly.Location,
                    typeof(System.Linq.Enumerable).Assembly.Location,
                    typeof(System.Text.RegularExpressions.Regex).Assembly.Location,
                    typeof(DynamicAttribute).Assembly.Location,
                    typeof(System.Dynamic.ExpandoObject).Assembly.Location,
                    typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location,
                }),
                Code);

            if(results.Errors.HasErrors)
            {
                results.Errors.Trace();
                return null;
            }

            var type = results.CompiledAssembly.GetType("CSharpModelEvaluator");
            dynamic modelBuilder = Activator.CreateInstance(type);
            return modelBuilder.GetModel();
        }


        private static string LoadClassTemplate()
        {
            var type = typeof (CSharpModelProvider);
            using (var reader = new StreamReader(type.Assembly.GetManifestResourceStream(type, "CSharpModelEvaluatorTemplate.cs")))
                return reader.ReadToEnd();
        }
    }
}