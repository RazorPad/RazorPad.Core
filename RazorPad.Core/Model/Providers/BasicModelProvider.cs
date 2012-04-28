using System.ComponentModel.Composition;
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

                if(changed)
                    TriggerModelChanged();
            }
        }
        private dynamic _model;

        public BasicModelProvider(object model = null)
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
            Model = serializer.DeserializeObject(serialized);
        }

        protected override dynamic RebuildModel()
        {
            return Model;
        }


        [Export(typeof(IModelProviderFactory))]
        public class BasicModelProviderFactory : IModelProviderFactory
        {
            public IModelProvider Create(dynamic model = null)
            {
                return new BasicModelProvider(model: model);
            }
        }
    }
}