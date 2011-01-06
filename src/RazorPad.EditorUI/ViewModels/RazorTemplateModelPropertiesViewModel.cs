using System;
using System.ComponentModel;
using RazorPad.Framework;

namespace RazorPad.ViewModels
{
    public class RazorTemplateModelPropertiesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Type TemplateModelType
        {
            get { return _templateModelType; }
            set
            {
                if (_templateModelType == value)
                    return;

                _templateModelType = value;
                LoadTemplateModelTypeProperties();
                OnPropertyChanged("TemplateModelType");
            }
        }
        private Type _templateModelType;


        public DynamicDictionary Properties { get; set; }


        public RazorTemplateModelPropertiesViewModel(Type templateModelType = null)
        {
            TemplateModelType = templateModelType ?? typeof(object);
            Properties = new DynamicDictionary();
            LoadTemplateModelTypeProperties();
        }

        private void LoadTemplateModelTypeProperties()
        {
            // TODO: Analyze template for additional properties
        }


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}