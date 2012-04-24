using System;
using System.Web.Script.Serialization;

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

        public override string Serialize()
        {
            return new JavaScriptSerializer().Serialize(Model);
        }

        public override void Deserialize(string serialized)
        {
            var serializer = new JavaScriptSerializer();

            if (ModelType == typeof(object))
                Model = serializer.DeserializeObject(serialized);
            else
                Model = serializer.Deserialize(serialized, ModelType);
        }

        protected override dynamic RebuildModel()
        {
            return Model;
        }
    }
}