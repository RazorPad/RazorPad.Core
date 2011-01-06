using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace RazorPad.Framework
{
    public class DynamicDictionary : DynamicObject, IDictionary, IDictionary<string,object>
    {
        readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public void Add(KeyValuePair<string, object> item)
        {
            this[item.Key] = item.Value;
        }

        public bool Contains(object key)
        {
            return key is string && _dictionary.ContainsKey(((string)key).ToLower());
        }

        public void Add(object key, object value)
        {
            this[key as string] = value;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void Remove(object key)
        {
            if(key is string)
                _dictionary.Remove(((string)key).ToLower());
            else
                throw new NotSupportedException("Only string keys are allowed");
        }

        object IDictionary.this[object key]
        {
            get { return _dictionary[key as string]; }
            set
            {
                var keyString = key as string;
                if(string.IsNullOrEmpty(keyString))
                    throw new ArgumentNullException("key");

                _dictionary[keyString.ToLower()] = value;
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if(!_dictionary.ContainsKey(item.Key.ToLower())) return false;

            return _dictionary[item.Key.ToLower()].Equals(item.Value);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _dictionary.Remove(item.Key.ToLower());
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        ICollection IDictionary.Values
        {
            get { return _dictionary.Values; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            if (!_dictionary.TryGetValue(name, out result))
                result = string.Empty;

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name.ToLower()] = value;
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key.ToLower());
        }

        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out object value)
        {
            var hasKey = ContainsKey(key);
            value = hasKey ? this[key] : default(object);
            return hasKey;
        }

        public object this[string key]
        {
            get { return _dictionary[key.ToLower()]; }
            set
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                _dictionary[key.ToLower()] = value;
            }
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        ICollection IDictionary.Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _dictionary.Values; }
        }
    }
}