using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Coralcode.Framework.Aspect.UnityExtension
{
    public class PerHttpLifetimeManager : LifetimeManager
    {
        [ThreadStatic]
        private static Dictionary<Guid, object> _values;
        private readonly Guid _key;

        public PerHttpLifetimeManager()
        {
            _key = Guid.NewGuid();
        }

        public override object GetValue()
        {
            EnsureValues();
            return _values.ContainsKey(_key) ? _values[_key] : null;
        }

        public override void SetValue(object newValue)
        {
            EnsureValues();
            _values[_key] = newValue;
        }

        public override void RemoveValue()
        {

        }

        private static void EnsureValues()
        {
            if (_values == null)
            {
                _values = new Dictionary<Guid, object>();
            }
        }
    }
}
