using System;
using System.Collections.Generic;
using System.Diagnostics;
using RazorPad.Framework;

namespace RazorPad
{
    public abstract class ModelProvider : IModelProvider
    {
        public event EventHandler<RazorPadErrorEventArgs> Error
        {
            add { _errorHandlers += value; }
            remove { _errorHandlers -= value; }
        }
        private event EventHandler<RazorPadErrorEventArgs> _errorHandlers;

        public event EventHandler ModelChanged
        {
            add { _modelChangedHandlers += value; }
            remove { _modelChangedHandlers -= value; }
        }
        private event EventHandler _modelChangedHandlers;


        public virtual dynamic GetModel()
        {
            dynamic model = null;

            try
            {
                model = RebuildModel();

                if (model is IDictionary<string, object>)
                    model = new DynamicDictionary((IDictionary<string, object>)model);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error rebuilding model: {0}", ex);
                TriggerError(new RazorPadError(ex));
            }

            return model ?? new DynamicDictionary();
        }

        protected void TriggerError(RazorPadError error)
        {
            if (_errorHandlers != null)
                _errorHandlers(this, new RazorPadErrorEventArgs(error));
        }

        protected void TriggerModelChanged()
        {
            if (_modelChangedHandlers != null)
                _modelChangedHandlers(this, EventArgs.Empty);
        }

        protected abstract dynamic RebuildModel();

        public abstract string Serialize();
        public abstract void Deserialize(string serialized);
    }
}
