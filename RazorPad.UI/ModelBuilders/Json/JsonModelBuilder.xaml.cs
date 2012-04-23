using System.ComponentModel.Composition;
using RazorPad.Providers;

namespace RazorPad.UI.Json
{
    [Export(typeof(ModelBuilder))]
    public partial class JsonModelBuilder : ModelBuilder
    {
        protected JsonModelProvider ViewModel
        {
            get { return (JsonModelProvider)ModelProvider; }
        }

        public JsonModelBuilder()
        {
            InitializeComponent();
            ModelProvider = new JsonModelProvider();
        }
    }
}
