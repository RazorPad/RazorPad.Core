using RazorPad.Providers;

namespace RazorPad.UI.Json
{
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
