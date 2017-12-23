using System;
using Coralcode.Framework.GenericsFactory;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.MessageBus
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class MessageAttribute : StrategyAttribute
    {
        public MessageAttribute(string description) : base(description)
        {
        }
    }
    
}
