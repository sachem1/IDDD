using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.WCF.Exception
{
    public class WcfException : WebFaultException<ResultErrorMessage>
    {
        public WcfException(string errorMessage)
            : this(new ResultErrorMessage(ResultState.Fail, errorMessage), HttpStatusCode.OK)
        {
            
        }

        public WcfException(ResultErrorMessage detail, HttpStatusCode statusCode) : base(detail, statusCode)
        {
        }

        public WcfException(ResultErrorMessage detail, HttpStatusCode statusCode, IEnumerable<Type> knownTypes) : base(detail, statusCode, knownTypes)
        {
        }

        protected WcfException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}