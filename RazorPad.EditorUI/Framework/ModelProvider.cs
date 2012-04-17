using System;

namespace RazorPad.Framework
{
    public abstract class ModelProvider : NotifyPropertyChanged, IModelProvider
    {
        public event EventHandler ModelChanged
        {
            add { _modelChanged += value; }
            remove { _modelChanged -= value; }
        }
        private event EventHandler _modelChanged;


        public Type ModelType
        {
            get { return _modelType; }
            set
            {
                if (_modelType == value)
                    return;

                _modelType = value;
                TriggerPropertyChanged("ModelType");
            }
        }
        private Type _modelType;


        protected ModelProvider(Type modelType)
        {
            ModelType = modelType ?? typeof(object);
        }


        public abstract dynamic GetModel();


        public void TriggerModelChanged()
        {
            if (_modelChanged != null)
                _modelChanged(this, EventArgs.Empty);
        }
    }
}