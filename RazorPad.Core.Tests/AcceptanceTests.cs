using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Compilation;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        private TemplateCompiler _templateCompiler;

        [TestInitialize]
        public void TestInitialize()
        {
            _templateCompiler = new TemplateCompiler();
        }


        [TestMethod]
        public void ShouldSupportFunctions()
        {
            string template = LoadResource("AcceptanceTests.Functions.cshtml");

            var results = _templateCompiler.Execute(template);

            string output = LoadResource("AcceptanceTests.Functions.cshtml.output");
            Assert.AreEqual(results.Trim(), output);
        }


        [TestMethod]
        public void ShouldSupportHelpers()
        {
            string template = LoadResource("AcceptanceTests.Helpers.cshtml");

            var results = _templateCompiler.Execute(template);

            string output = LoadResource("AcceptanceTests.Helpers.cshtml.output");
            Assert.AreEqual(results.Trim(), output);
        }


        [TestMethod]
        public void ShouldSupportSimplerendering()
        {
            string template = LoadResource("AcceptanceTests.SimpleRendering.cshtml");

            var results = _templateCompiler.Execute(template);

            string output = LoadResource("AcceptanceTests.SimpleRendering.cshtml.output");
            Assert.AreEqual(results.Trim(), output);
        }


        private string LoadResource(string name)
        {
            var type = GetType();
            using (var reader = new StreamReader(type.Assembly.GetManifestResourceStream(type, name)))
                return reader.ReadToEnd();
        }
    }
}
