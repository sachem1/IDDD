using System;
using System.ServiceModel.Configuration;
using Coralcode.Framework.WCF.Aspect;

namespace Coralcode.Framework.WCF.Extensions
{
    public class ExceptionBehaviorExtensionElement : BehaviorExtensionElement
    {

        public override Type BehaviorType
        {
            get { return typeof(ExceptionHandler); }
        }

        protected override object CreateBehavior()
        {
            return new ExceptionHandler();
        }
    }
}
