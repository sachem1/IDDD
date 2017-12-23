using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.WCF.WcfService
{
    public class WcfException : WebFaultException<ResultMessage>
    {
        public WcfException(string errorMessage)
            : this(new ResultMessage
            {
                Message = errorMessage
            },HttpStatusCode.OK)
        {
            
        }

        public WcfException(ResultMessage detail, HttpStatusCode statusCode) : base(detail, statusCode)
        {
        }

        public WcfException(ResultMessage detail, HttpStatusCode statusCode, IEnumerable<Type> knownTypes) : base(detail, statusCode, knownTypes)
        {
        }

        protected WcfException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}