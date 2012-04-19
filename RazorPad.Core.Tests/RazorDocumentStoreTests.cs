using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Providers;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class RazorDocumentStoreTests
    {
        private RazorDocumentStore _store;
        private XDocument _demoDoc;

        [TestInitialize]
        public void TestInitialize()
        {
            _store = new RazorDocumentStore(null);
            _demoDoc = XDocument.Parse(@"
                        <RazorDocument>
                            <Metadata>
                                <Title>Hello World!</Title>
                                <Created>2012-04-19 12:42:19</Created>
                                <LastUpdated>2012-04-19 20:12:50</LastUpdated>
                            </Metadata>

                            <Model Provider=""Json"">
                            <![CDATA[
                                { Name: ""RazorPad"" }
                            ]]>
                            </Model>

                            <Template BaseClass=""RazorPad.Compilation.TemplateBase"">
                            <![CDATA[
                                <h1>Hello, @Model.Name!</h1>
                            ]]>
                            </Template>

                        </RazorDocument>")
        }

        [TestMethod]
        public void ShouldLoadRazorDocument()
        {
            var document = _store.Load(_demoDoc);

            Assert.AreEqual(document.Template.Trim(), "<h1>Hello, @Model.Name!</h1>");
            Assert.AreEqual(document.Metadata["Title"], "Hello World!");
            Assert.IsInstanceOfType(document.ModelProvider, typeof(JsonModelProvider));

            var model = document.GetModel();
            Assert.AreEqual(model.Name, "RazorPad");
        }
    }
}
