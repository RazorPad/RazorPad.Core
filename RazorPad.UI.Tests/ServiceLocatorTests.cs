using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Persistence;
using RazorPad.ViewModels;

namespace RazorPad.UI
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServiceLocator.Initialize();
        }

        [TestMethod]
        public void CanGetMainViewModel()
        {
            var viewModel = ServiceLocator.Get<MainWindowViewModel>();
            Assert.IsNotNull(viewModel);
        }

        [TestMethod]
        public void CanGetModelProviders()
        {
            var providers = ServiceLocator.Get<ModelProviders>();
            Assert.IsNotNull(providers);
            Assert.AreNotEqual(0, providers.Providers.Count());
        }

        [TestMethod]
        public void CanGetModelBuilders()
        {
            var builders = ServiceLocator.Get<ModelBuilders.ModelBuilders>();
            Assert.IsNotNull(builders);
            Assert.AreNotEqual(0, builders.Builders.Count());
        }

        [TestMethod]
        public void CanGetRazorDocumentManager()
        {
            var manager = ServiceLocator.Get<RazorDocumentManager>();
            Assert.IsNotNull(manager);
        }
    }
}
