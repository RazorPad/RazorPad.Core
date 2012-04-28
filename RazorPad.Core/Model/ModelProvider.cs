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


        public virtual dynamic GetModel()
        {
            var model = RebuildModel();

            if (model is IDictionary<string, object>)
                model = new DynamicDictionary((IDictionary<string, object>)model);

            return model;
        }

        protected void TriggerModelChanged()
        {
            if (_modelChanged != null)
                _modelChanged(this, EventArgs.Empty);
        }

        protected abstract dynamic RebuildModel();

        public abstract string Serialize();
        public abstract void Deserialize(string serialized);
    }
}