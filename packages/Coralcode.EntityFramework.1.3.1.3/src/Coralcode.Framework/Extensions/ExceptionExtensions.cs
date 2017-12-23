using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Extensions
{
    public static class ExceptionExtensions
    {
        public static string DataToJson(this Exception ex)
        {
            if (ex.Data != null && ex.Data.Count != 0)
                return JsonConvert.SerializeObject(ex.Data);
            return ex.Message;
        }
    }
}
