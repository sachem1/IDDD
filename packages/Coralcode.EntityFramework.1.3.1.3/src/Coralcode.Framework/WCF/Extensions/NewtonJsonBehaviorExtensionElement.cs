using System;
using System.ServiceModel.Configuration;
using Coralcode.Framework.WCF.Json;

namespace Coralcode.Framework.WCF.Extensions
{
    public class NewtonJsonBehaviorExtensionElement : BehaviorExtensionElement
    {

        public override Type BehaviorType
        {
            get { return typeof(NewtonsoftJsonBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new NewtonsoftJsonBehavior();
        }
    }
}
