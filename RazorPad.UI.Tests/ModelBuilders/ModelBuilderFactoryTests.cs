using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Providers;

namespace RazorPad.UI.ModelBuilders
{
    [TestClass]
    public class ModelBuilderFactoryTests
    {
        private ModelBuilderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new ModelBuilderFactory(new[] {new Json.JsonModelBuilder()});
        }

        [TestMethod]
        public void ShouldInstantiateModelBuilderByNameExcludingModelBuilderSuffix()
        {
            var builder = _factory.Create("Json");
            Assert.IsInstanceOfType(builder, typeof(Json.JsonModelBuilder));
        }

        [TestMethod]
        public void ShouldInstantiateModelBuilderByModelProviderOfSameNameExcludingModelProviderSuffix()
        {
            var jsonModelProvider = new JsonModelProvider();
            var builder = _factory.Create(jsonModelProvider);
            Assert.IsInstanceOfType(builder, typeof(Json.JsonModelBuilder));
        }

        [TestMethod]
        public void ShouldPopulateModelProviderPropertyWhenCreateIsCalledWithModelProviderArgument()
        {
            var jsonModelProvider = new JsonModelProvider();
            var builder = _factory.Create(jsonModelProvider);
            Assert.AreSame(jsonModelProvider, builder.ModelProvider);
        }
    }
}
