using System.Windows;
using System.Windows.Forms;
using RazorPad.Framework;

namespace RazorPad.ModelProviders
{
    /// <summary>
    /// Interaction logic for RazorTemplateModelProperties.xaml
    /// </summary>
    public partial class RazorTemplateModelProperties
    {
        protected PropertyGridModelProvider ViewModel
        {
            get { return (PropertyGridModelProvider) ModelProvider; }
        }

        protected DynamicDictionary PropertyDictionary { get; set; }

        public RazorTemplateModelProperties()
        {
            InitializeComponent();
            PropertyDictionary = new DynamicDictionary();
            ModelProvider = new PropertyGridModelProvider();
        }

        private void OnAddPropertyClick(object sender, RoutedEventArgs e)
        {
            string newPropertyName = NewPropertyName.Text;

            if (string.IsNullOrWhiteSpace(newPropertyName))
                return;

            PropertyDictionary.Add(newPropertyName, string.Empty);

            PropertyGrid.Refresh();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePropertyGrid(ViewModel != null ? ViewModel.Properties : null);
        }

        private void UpdatePropertyGrid(DynamicDictionary newDictionary)
        {
            if (newDictionary == null)
            {
                newDictionary = new DynamicDictionary();
            }
            else
            {
                foreach (var item in PropertyDictionary)
                {
                    if (!newDictionary.ContainsKey(item.Key))
                        newDictionary.Add(item);
                }
            }

            PropertyDictionary = newDictionary;
            PropertyGrid.SelectedObject = new DictionaryPropertyGridAdapter(PropertyDictionary);
        }

        private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ViewModel.TriggerModelChanged();
        }
    }
}
