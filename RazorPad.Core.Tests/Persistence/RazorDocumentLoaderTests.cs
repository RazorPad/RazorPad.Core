using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Persistence;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class RazorDocumentLoaderTests
    {
        private RazorDocumentManager _manager;

        [TestInitialize]
        public void TestInitialize()
        {
            _manager = new RazorDocumentManager(new XmlRazorDocumentSource());
        }

        [TestMethod]
        public void ShouldLoadXmlRazorDocument()
        {
            var document = _manager.Parse("<RazorDocument><References><Reference>System.Awesome</Reference></References><Template><![CDATA[<h1>Hello, World!</h1>]]></Template></RazorDocument>");

            Assert.AreEqual("<h1>Hello, World!</h1>", document.Template);
            Assert.AreEqual("System.Awesome", document.References.Single());
        }

        [TestMethod]
        public void ShouldLoadCSharpRazorTemplate()
        {
            var document = _manager.Parse(@"<h1>Hello, World!</h1>");
            Assert.AreEqual("<h1>Hello, World!</h1>", document.Template.Trim());
        }
    }
}