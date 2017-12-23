using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Cache.Redis;
using Coralcode.Framework.Log;
using Coralcode.Framework.Modules;
using Coralcode.Framework.Reflection;
using Coralcode.Framework.Data;

namespace Coralcode.Framework
{
    public class CoreModule : CoralModule
    {

        public override void Prepare()
        {
            UnityService.RegisterInstance(typeof(ICache), CacheFactory.GetLocalCache());
            if(!UnityService.HasRegistered(typeof(IDbConfigLoader)))
                UnityService.RegisterInstance(typeof(IDbConfigLoader), new DefaultDbConfigLoader());
        }

        public override void Install()
        {
            //UnityService.Resolve<DbFactory>().Init();
        }

        public override void Installed()
        {
        }

        public override void UnInstall()
        {
        }
    }
}
