using System;

namespace RazorPad.Providers
{
    public class BasicModelProvider : ModelProvider
    {
        public dynamic Model
        {
            get { return _model; }
            set
            {
                var changed = _model != value;

                _model = value;
                ModelType = (_model == null) ? typeof(object) : _model.GetType();

                if(changed)
                    TriggerModelChanged();
            }
        }
        private dynamic _model;

        public BasicModelProvider(Type modelType = null, object model = null)
            : base(modelType)
        {
            Model = model;
        }

        protected override dynamic RebuildModel()
        {
            return Model;
        }
    }
}