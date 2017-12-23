using Coralcode.Framework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Aspect.Unity;

namespace Coralcode.Framework.Data
{
    public abstract class DbModule : CoralModule
    {
        private List<Type> _entityTypes;

        public virtual List<Type> EntityTypes
        {
            get
            {
                if (_entityTypes != null)
                    return _entityTypes;
                _entityTypes = Types.Where(type => Entity.IsChild(type) && !IgnoreAttribute.IsDefined(type) && !type.IsAbstract).ToList();
                return _entityTypes;
            }
        }

        internal static Func<Type, bool> IsDbModule
        {
            get { return item => item.IsSubclassOf(typeof(DbModule)) && !item.IsAbstract; }
        }

        public override void Install()
        {
            UnityService.Resolve<DbFactory>().Init();
        }

    }
}
