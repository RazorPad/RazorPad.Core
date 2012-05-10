namespace RazorPad.UI
{
    public class ModelBuilderFactory<TModelProvider, TModelBuilder> : IModelBuilderFactory
        where TModelProvider : class, IModelProvider
        where TModelBuilder : ModelBuilder, new()
    {
        public bool CanBuild(IModelProvider provider)
        {
            return provider is TModelProvider;
        }

        public IModelBuilder Build(IModelProvider provider = null)
        {
            var modelProvider = provider as TModelProvider;
            dynamic builder = new TModelBuilder();

            var viewModel = ViewModel(modelProvider);
            if (viewModel != null)
                builder.DataContext = viewModel;

            builder.InitializeComponent();
            return builder;
        }

        protected virtual dynamic ViewModel(TModelProvider provider)
        {
            return null;
        }
    }
}