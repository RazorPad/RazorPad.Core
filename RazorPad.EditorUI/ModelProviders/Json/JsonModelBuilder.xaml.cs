using RazorPad.Framework;

namespace RazorPad.ModelProviders.Json
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
