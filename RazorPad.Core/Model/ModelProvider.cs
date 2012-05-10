using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public ObservableCollection<RazorPadError> Errors
        {
            get { return _errors; }
        }
        private readonly ObservableCollection<RazorPadError> _errors = new ObservableCollection<RazorPadError>();

        public virtual dynamic GetModel()
        {
            dynamic model = null;

            try
            {
                Errors.Clear();

                model = RebuildModel();

                if (model is IDictionary<string, object>)
                    model = new DynamicDictionary((IDictionary<string, object>)model);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error rebuilding model: {0}", ex);
                Errors.Add(new RazorPadError(ex));
            }

            return model ?? new DynamicDictionary();
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
