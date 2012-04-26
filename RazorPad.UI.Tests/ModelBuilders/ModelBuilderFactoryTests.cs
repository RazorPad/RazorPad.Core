using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Providers;
using RazorPad.UI.Json;

namespace RazorPad.UI.ModelBuilders
{
    [TestClass]
    public class ModelBuilderFactoryTests
    {
        private ModelBuilders _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new ModelBuilders(new[] {new JsonModelBuilderFactory()});
        }

        [TestMethod]
        public void ShouldInstantiateModelBuilderByModelProviderOfSameNameExcludingModelProviderSuffix()
        {
            var jsonModelProvider = new JsonModelProvider();
            var builder = _factory.Create(jsonModelProvider);
            Assert.IsInstanceOfType(builder, typeof(JsonModelBuilder));
        }

        [TestMethod]
        public void ShouldPopulateModelProviderPropertyWhenCreateIsCalledWithModelProviderArgument()
        {
            var jsonModelProvider = new JsonModelProvider();
            var builder = _factory.Create(jsonModelProvider);
            dynamic dataContext = builder.DataContext;
            Assert.AreSame(jsonModelProvider, dataContext.ModelProvider);
        }
    }
}
