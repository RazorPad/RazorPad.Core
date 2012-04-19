using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Persistence;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class RazorDocumentLoaderTests
    {
        private RazorDocumentLoader _loader;

        [TestInitialize]
        public void TestInitialize()
        {
            _loader = new RazorDocumentLoader();
        }

        [TestMethod]
        public void ShouldLoadXmlRazorDocument()
        {
            var document = _loader.Load("<RazorDocument><Template><![CDATA[<h1>Hello, World!</h1>]]></Template></RazorDocument>");
            Assert.AreEqual("<h1>Hello, World!</h1>", document.Template);
        }

        [TestMethod]
        public void ShouldLoadCSharpRazorTemplate()
        {
            var document = _loader.Load(@"<h1>Hello, World!</h1>");
            Assert.AreEqual("<h1>Hello, World!</h1>", document.Template.Trim());
        }
    }
}