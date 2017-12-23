using System;

namespace Coralcode.Framework.Exceptions
{
    public class NotFoundException :CoralException
    {
        public NotFoundException(string message) :
            base(message)
        {
            
        }
    }
}
