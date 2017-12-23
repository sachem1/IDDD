using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Web
{
    public static class WebApiExtensions
    {
        public static string GetSingleHeadValue(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> values;
            if (request.Headers.TryGetValues(key, out values))
            {
                return values.FirstOrDefault();
            }
            return string.Empty;
        }
    }
}
