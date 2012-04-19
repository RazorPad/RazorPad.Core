using System;
using System.Collections.Generic;
using RazorPad.Framework;

namespace RazorPad
{
    public abstract class ModelProvider : IModelProvider
    {
        public event EventHandler ModelChanged
        {
            add { _modelChanged += value; }
            remove { _modelChanged -= value; }
        }
        private event EventHandler _modelChanged;

        public Type ModelType { get; set; }


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