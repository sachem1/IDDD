using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.Cache
{
    public static class CacheExtensions
    {
        public static List<TEntity> GetValues<TEntity>(this ICache cache, string contextKey)
        {
            return cache.Get<TEntity>(contextKey)?.Values.ToList();
        }
    }
}
