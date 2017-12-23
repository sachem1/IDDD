using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Coralcode.Framework.Serializers.Dynamic
{
    [Serializable]
    public class DynamicDictionary : DynamicObject
    {
        private Dictionary<string, dynamic> _dictionary = new Dictionary<string, dynamic>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            if (!_dictionary.ContainsKey(name))
            {
                _dictionary.Add(name, new DynamicDictionary());
            }
            return _dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var key = binder.Name;
            if (_dictionary.ContainsKey(key))
                _dictionary[key] = value;
            else
            {
                _dictionary.Add(key, value);
            }


            return true;
        }

        public Dictionary<string, dynamic> Dictionary
        {
            get { return _dictionary; }
        }

        public void AddMember(string name, dynamic value)
        {
            _dictionary.Add(name, value);
        }
    }
}
