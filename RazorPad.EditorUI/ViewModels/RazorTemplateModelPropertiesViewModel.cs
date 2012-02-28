using System;
using System.ComponentModel;
using RazorPad.Framework;

namespace RazorPad.ViewModels
{
    public class RazorTemplateModelPropertiesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> PropertiesUpdated;

        public Type TemplateModelType
        {
            get { return _templateModelType; }
            set
            {
                if (_templateModelType == value)
                    return;

                _templateModelType = value;
                OnPropertyChanged("TemplateModelType");
            }
        }
        private Type _templateModelType;


        public DynamicDictionary Properties
        {
            get { return _properties; }
            set
            {
                if(_properties == value)
                    return;

                _properties = value;
                OnPropertyChanged("Properties");
            }
        }
        private DynamicDictionary _properties;


        public RazorTemplateModelPropertiesViewModel(Type templateModelType = null)
        {
            TemplateModelType = templateModelType ?? typeof(object);
            Properties = new DynamicDictionary();
        }


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void TriggerPropertyChanged()
        {
            if (PropertiesUpdated != null)
                PropertiesUpdated(this, EventArgs.Empty);
        }
    }
}