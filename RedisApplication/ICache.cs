using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisApplication
{
    public interface ICache:IDisposable
    {
        T Get<T>(string contextKey, string dataKey);

        T Get<T>(string contextKey, string dataKey,Func<T,int> action);
    }
}
