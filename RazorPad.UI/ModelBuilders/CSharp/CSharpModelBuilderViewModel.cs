namespace RazorPad.UI.ModelBuilders.CSharp
{
    public class CSharpModelBuilderViewModel : ModelBuilderViewModel<CSharpModelProvider>
    {
        public string CSharp
        {
            get { return ModelProvider.Code; }
            set { ModelProvider.Code = value; }
        }

        public CSharpModelBuilderViewModel(CSharpModelProvider modelProvider = null) 
            : base(modelProvider ?? new CSharpModelProvider())
        {
        }

        protected override void OnModelChanged()
        {
            OnPropertyChanged("CSharp");
        }
    }
}
