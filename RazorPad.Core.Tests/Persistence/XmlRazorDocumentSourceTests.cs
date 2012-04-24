using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Persistence;
using RazorPad.Providers;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class XmlRazorDocumentSourceTests
    {
        private XmlRazorDocumentSource _documentSource;
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
            _documentSource = new XmlRazorDocumentSource();
        }

        [TestMethod]
        public void ShouldLoadEmptyDocument()
        {
            var document = _documentSource.Parse("<RazorDocument />");

           Assert.IsNotNull(document);
        }

        [TestMethod]
        public void ShouldLoadRazorDocument()
        {
            var document = _documentSource.Parse(DemoDocument);

            Assert.AreEqual("<h1>Hello, @Model.Name!</h1>", document.Template.Trim());
            Assert.AreEqual("Hello World!", document.Metadata["Title"]);
            Assert.IsInstanceOfType(document.ModelProvider, typeof(JsonModelProvider));

            dynamic model = document.GetModel();
            Assert.AreEqual("RazorPad", model.Name);
        }

        [TestMethod]
        public void ShouldSaveRazorDocument()
        {
            const string expectedModel = "{Name:'RazorPad'}";
            var document = new RazorDocument
                               {
                                   ModelProvider = new JsonModelProvider(json: expectedModel),
                                   Template = "<h1>Test!</h1>",
                               };
            document.Metadata.Add("Title", "Test Document");

            XDocument savedXml;
            using (var stream = new MemoryStream())
            {
                _documentSource.Save(document, stream);
                stream.Seek(0, SeekOrigin.Begin);
                savedXml = XDocument.Load(stream);
            }

            var modelElement = savedXml.Root.Element("Model");
            Assert.AreEqual("Json", modelElement.Attribute("Provider").Value);
            Assert.AreEqual(expectedModel, modelElement.Value);
        }

        [TestMethod]
        public void ShouldSaveAndLoadRazorDocument()
        {
            const string expectedModel = "{Name:'RazorPad'}";
            var originalDocument = new RazorDocument
                               {
                                   ModelProvider = new JsonModelProvider(json: expectedModel),
                                   Template = "<h1>Test!</h1>",
                               };
            originalDocument.Metadata.Add("Title", "Test Document");

            RazorDocument rehydratedDocument;
            using (var stream = new MemoryStream())
            {
                _documentSource.Save(originalDocument, stream);
                stream.Seek(0, SeekOrigin.Begin);
                rehydratedDocument = _documentSource.Load(stream);
            }

            Assert.AreEqual(originalDocument.Metadata["Title"], rehydratedDocument.Metadata["Title"]);
            Assert.AreEqual(originalDocument.Template, rehydratedDocument.Template);
            Assert.AreEqual(((JsonModelProvider)originalDocument.ModelProvider).Json, 
                            ((JsonModelProvider)rehydratedDocument.ModelProvider).Json);
        }
    }
}
