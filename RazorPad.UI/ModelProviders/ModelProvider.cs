using System;
using System.Collections.Generic;
using RazorPad.Framework;

namespace RazorPad.UI
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


        public virtual dynamic GetModel()
        {
            var model = RebuildModel();

            if (model is IDictionary<string, object>)
                model = new DynamicDictionary((IDictionary<string, object>)model);

            return model;
        }

        public void TriggerModelChanged()
        {
            if (_modelChanged != null)
                _modelChanged(this, EventArgs.Empty);
        }

        protected abstract dynamic RebuildModel();
    }
}