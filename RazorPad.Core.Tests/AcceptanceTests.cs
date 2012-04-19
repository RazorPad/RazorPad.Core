using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Compilation;
using RazorPad.Persistence;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        private TemplateCompiler _templateCompiler;
        private RazorDocumentLoader _loader;

        [TestInitialize]
        public void TestInitialize()
        {
            _loader = new RazorDocumentLoader();
            _templateCompiler = new TemplateCompiler();
        }


        [TestMethod]
        public void ShouldSupportFunctions()
        {
            var document = LoadDocument("AcceptanceTests.Functions.cshtml");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.Functions.cshtml.output");
            Assert.AreEqual(results.Trim(), output);
        }


        [TestMethod]
        public void ShouldSupportHelpers()
        {
            var document = LoadDocument("AcceptanceTests.Helpers.cshtml");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.Helpers.cshtml.output");
            Assert.AreEqual(results.Trim(), output);
        }


        [TestMethod]
        public void ShouldSupportSimpleRendering()
        {
            var document = LoadDocument("AcceptanceTests.SimpleRendering.cshtml");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.SimpleRendering.cshtml.output");
            Assert.AreEqual(results.Trim(), output);
        }


        [TestMethod]
        public void ShouldSupportHelloWorld()
        {
            var document = LoadDocument("AcceptanceTests.HelloWorld.razorpad");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.HelloWorld.razorpad.output");
            Assert.AreEqual(results.Trim(), output);
        }



        private RazorDocument LoadDocument(string name)
        {
            using (var reader = GetResourceStream(name))
                return _loader.Load(reader);
        }

        private string LoadResource(string name)
        {
            using (var stream = GetResourceStream(name))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private Stream GetResourceStream(string resourceName)
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type, resourceName);
        }
    }
}
