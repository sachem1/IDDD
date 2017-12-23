using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Aspect.InstanceProviders;
using Coralcode.Framework.WCF.Aspect;

namespace Coralcode.Framework.WCF.Extensions
{
    public class InstanceProviderExtensionElement : BehaviorExtensionElement
    {

        public override Type BehaviorType
        {
            get { return typeof(UnityInstanceProviderServiceBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new UnityInstanceProviderServiceBehavior();
        }
    }
}
