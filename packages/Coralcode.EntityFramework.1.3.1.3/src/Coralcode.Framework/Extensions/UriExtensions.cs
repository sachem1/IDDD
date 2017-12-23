using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Extensions
{
    public static class UriExtensions
    {
        public static string GetUrlHost(this Uri uri)
        {
            if (uri.Port == 0)
                return string.Format("{0}://{1}", uri.Scheme, uri.Host);
            return string.Format("{0}://{1}:{2}", uri.Scheme, uri.Host, uri.Port);
        }
    }
}
