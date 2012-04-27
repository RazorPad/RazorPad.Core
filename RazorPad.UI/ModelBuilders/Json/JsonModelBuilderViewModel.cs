using RazorPad.Providers;

namespace RazorPad.UI.ModelBuilders.Json
{
    public class JsonModelBuilderViewModel : ModelBuilderViewModel<JsonModelProvider>
    {
        public string Json
        {
            get { return ModelProvider.Json; }
            set { ModelProvider.Json = value; }
        }

        public JsonModelBuilderViewModel(JsonModelProvider modelProvider = null) 
            : base(modelProvider ?? new JsonModelProvider())
        {
            modelProvider.ModelChanged += (sender, args) => OnModelChanged();
        }

        protected override void OnModelChanged()
        {
            OnPropertyChanged("Json");
        }
    }
}
