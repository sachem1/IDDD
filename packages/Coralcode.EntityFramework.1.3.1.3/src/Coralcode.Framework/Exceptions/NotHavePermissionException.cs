using System;

namespace Coralcode.Framework.Exceptions
{
    public class NotHavePermissionException : CoralException
    {
        public NotHavePermissionException() 
            : base("无访问权限")
        {
        }
    }
}
