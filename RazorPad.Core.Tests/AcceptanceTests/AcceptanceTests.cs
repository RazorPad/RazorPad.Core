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
        private RazorDocumentManager _manager;

        [TestInitialize]
        public void TestInitialize()
        {
            _manager = new RazorDocumentManager(new XmlRazorDocumentSource());
            _templateCompiler = new TemplateCompiler();
        }

		
        [TestMethod]
        public void ShouldSupportFunctions()
        {
            var document = LoadDocument("AcceptanceTests.Functions.cshtml");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.Functions.cshtml.output");
            Assert.AreEqual(output, results.Trim());
        }

		
        [TestMethod]
        public void ShouldSupportHelpers()
        {
            var document = LoadDocument("AcceptanceTests.Helpers.cshtml");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.Helpers.cshtml.output");
            Assert.AreEqual(output, results.Trim());
        }

		
        [TestMethod]
        public void ShouldSupportSimpleRendering()
        {
            var document = LoadDocument("AcceptanceTests.SimpleRendering.cshtml");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.SimpleRendering.cshtml.output");
            Assert.AreEqual(output, results.Trim());
        }

		
        [TestMethod]
        public void ShouldSupportBasicRazorDocument()
        {
            var document = LoadDocument("AcceptanceTests.BasicRazorDocument.razorpad");

            var results = _templateCompiler.Execute(document);

            string output = LoadResource("AcceptanceTests.BasicRazorDocument.razorpad.output");
            Assert.AreEqual(output, results.Trim());
        }



        private RazorDocument LoadDocument(string name)
        {
            using (var reader = GetResourceStream(name))
                return _manager.Load(reader);
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
