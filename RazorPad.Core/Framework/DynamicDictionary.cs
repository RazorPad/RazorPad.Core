using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace RazorPad.Framework
{
    public class DynamicDictionary : DynamicObject, IDictionary, IDictionary<string, object>, INotifyPropertyChanged
    {
        readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Add(KeyValuePair<string, object> item)
        {
            this[item.Key] = item.Value;
        }

        public bool Contains(object key)
        {
            string actualKey = GetActualKey(key);
            return actualKey != null && _dictionary.ContainsKey(actualKey);
        }

        public void Add(object key, object value)
        {
            this[key as string] = value;
        }

        public void Clear()
        {
            _dictionary.Clear();
            OnPropertyChanged(string.Empty);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void Remove(object key)
        {
            if (key is string)
            {
                string actualKey = GetActualKey(key);
                _dictionary.Remove(actualKey);
                OnPropertyChanged(actualKey);
            }
            else
                throw new NotSupportedException("Only string keys are allowed");
        }

        object IDictionary.this[object key]
        {
            get
            {
                string actualKey = GetActualKey(key);

                if (actualKey == null)
                    return null;

                return _dictionary[actualKey];
            }
            set
            {
                var keyString = key as string;
                if (string.IsNullOrEmpty(keyString))
                    throw new ArgumentNullException("key");

                if (_dictionary[keyString] == value)
                    return;

                _dictionary[keyString] = value;
                OnPropertyChanged(keyString);
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            var obj = this[item.Key];
            return obj == item.Value;
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
            string actualKey = GetActualKey(item.Key);
            return _dictionary.Remove(actualKey);
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
            string name = GetActualKey(binder.Name);

            if (string.IsNullOrWhiteSpace(name) || !_dictionary.TryGetValue(name, out result))
                result = string.Empty;

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
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
            return _dictionary.Keys.Any(x => x.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public bool Remove(string key)
        {
            string actualKey = GetActualKey(key);
            return _dictionary.Remove(actualKey);
        }

        public bool TryGetValue(string key, out object value)
        {
            var hasKey = ContainsKey(key);
            value = hasKey ? this[key] : default(object);
            return hasKey;
        }

        public object this[string key]
        {
            get
            {
                string actualKey = GetActualKey(key);
                return _dictionary[actualKey];
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                string actualKey = GetActualKey(key) ?? key;

                if (_dictionary.ContainsKey(actualKey))
                {
                    if (_dictionary[actualKey] == value)
                        return;

                    _dictionary[actualKey] = value;
                }
                else
                {
                    _dictionary.Add(actualKey, value);
                }

                OnPropertyChanged(actualKey);
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


        private string GetActualKey(object improperlyCasedKey)
        {
            string key = improperlyCasedKey as string;

            if (key == null)
                return null;

            var actualKey =
                _dictionary.Keys
                    .SingleOrDefault(x => x.Equals(key, StringComparison.OrdinalIgnoreCase));

            return actualKey;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}