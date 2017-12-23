using System;
using System.Linq;
using System.Reflection;

namespace Coralcode.Framework.Exceptions
{
    public class CoralModuleException : CoralException
    {

        public CoralModuleException(ReflectionTypeLoadException exception) :
            base(exception.Message, exception)
        {
            Exceptions = exception.LoaderExceptions;
            Message = Exceptions.Aggregate(exception.Message, (r, s) => r + s.Message);
            HResult = 10102;
        }

        public CoralModuleException(Exception exception) :
            base(exception.Message)
        {
            HResult = 10102;
            Message = exception.Message;
        }

        public override string Message { get; }

        public Exception[] Exceptions { get; }

    }
}
