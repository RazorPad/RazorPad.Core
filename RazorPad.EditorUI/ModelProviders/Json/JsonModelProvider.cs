using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using RazorPad.Framework;

namespace RazorPad.ModelProviders.Json
{
    public class JsonModelProvider : ModelProvider
    {
        public string Json
        {
            get { return _json; }
            set
            {
                if (_json == value)
                    return;

                _json = value;
                TriggerModelChanged();
                TriggerPropertyChanged("Json");
            }
        }
        private string _json;

        public JsonModelProvider(Type modelType = null, string json = null)
            : base(modelType)
        {
            Json = json;
        }

        public override dynamic GetModel()
        {
            dynamic model;

            var serializer = new JavaScriptSerializer();

            var json = (string.IsNullOrWhiteSpace(Json)) ? "{}" : Json;
            var modelType = ModelType ?? typeof (object);

            if (modelType == typeof(object))
                model = serializer.DeserializeObject(json);
            else
                model = serializer.Deserialize(json, modelType);

            if (model is IDictionary<string, object>)
                model = new DynamicDictionary((IDictionary<string, object>) model);

            return model;
        }
    }
}