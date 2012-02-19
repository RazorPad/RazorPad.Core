using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RazorPad.DynamicModel
{
    public class JsonDynamicObject : DynamicObject
    {
        private IDictionary<string, object> _properties;

        public JsonDynamicObject(IDictionary<string, object> dictionary)
        {
            _properties = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _properties[binder.Name];

            if (result is IDictionary<string, object>)
            {
                result = new JsonDynamicObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<JsonDynamicObject>((result as ArrayList).ToArray().Select(x => new JsonDynamicObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }

            return _properties.ContainsKey(binder.Name);
        }

    }
}
