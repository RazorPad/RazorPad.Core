using System;
using System.Collections.Generic;
using RazorPad.Framework;

namespace RazorPad.ModelProviders
{
    public class PropertyGridModelProvider : ModelProvider
    {
        public DynamicDictionary Properties
        {
            get { return _properties; }
            set
            {
                if(_properties == value)
                    return;

                _properties = value;
                TriggerPropertyChanged("Properties");
            }
        }
        private DynamicDictionary _properties;

        public PropertyGridModelProvider(Type modelType = null, IDictionary<string, object> properties = null)
            : base(modelType)
        {
            Properties = new DynamicDictionary(properties);
        }

        public override dynamic GetModel()
        {
            return Properties;
        }
    }
}