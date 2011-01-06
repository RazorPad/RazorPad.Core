using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RazorPad.Framework
{
    public class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        IDictionary<string, object> _dictionary;
        string _key;

        internal DictionaryPropertyDescriptor(IDictionary<string,object> d, string key)
            : base(key, null)
        {
            _dictionary = d;
            _key = key;
        }

        public override Type PropertyType
        {
            get { return _dictionary[_key].GetType(); }
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}