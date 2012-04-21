using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Persistence;
using RazorPad.Providers;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class XmlRazorDocumentLoaderTests
    {
        private XmlRazorDocumentLoader _loader;
        private const string DemoDocument = @"
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

                        </RazorDocument>";

        [TestInitialize]
        public void TestInitialize()
        {
            _loader = new XmlRazorDocumentLoader();
        }

        [TestMethod]
        public void ShouldLoadEmptyDocument()
        {
            var document = _loader.Parse("<RazorDocument />");

           Assert.IsNotNull(document);
        }

        [TestMethod]
        public void ShouldLoadRazorDocument()
        {
            var document = _loader.Parse(DemoDocument);

            Assert.AreEqual("<h1>Hello, @Model.Name!</h1>", document.Template.Trim());
            Assert.AreEqual("Hello World!", document.Metadata["Title"]);
            Assert.IsInstanceOfType(document.ModelProvider, typeof(JsonModelProvider));

            dynamic model = document.GetModel();
            Assert.AreEqual("RazorPad", model.Name);
        }
    }
}
