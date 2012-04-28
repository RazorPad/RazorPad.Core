using RazorPad.Extensions.CSharp.ModelProvider;
using RazorPad.UI.ModelBuilders;

namespace RazorPad.Extensions.CSharp.ModelBuilder
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
            modelProvider.ModelChanged += (sender, args) => OnModelChanged();
        }

        protected override void OnModelChanged()
        {
            OnPropertyChanged("CSharp");
        }
    }
}
