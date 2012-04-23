using System.ComponentModel.Composition;
using RazorPad.Providers;
using RazorPad.UI.ModelBuilders.Json;

namespace RazorPad.UI.Json
{
    [Export(typeof(ModelBuilder))]
    public partial class JsonModelBuilder : ModelBuilder
    {
        public JsonModelBuilder()
        {
            InitializeComponent();
        }
    }

    public class JsonModelBuilderBuilder : IModelBuilderBuilder
    {
        public ModelBuilder Build(IModelProvider provider = null)
        {
            var modelProvider = provider as JsonModelProvider;
            var viewModel = new JsonModelBuilderViewModel(modelProvider);
            return new JsonModelBuilder {DataContext = viewModel};
        }
    }
}
