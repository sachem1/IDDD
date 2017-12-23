using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Contexts
{
    public interface IContextOperation
    {
        /// <summary>
        /// 定时刷新LastTime
        /// </summary>
        void Refresh();

        /// <summary>
        /// 扫描Redis
        /// </summary>
        /// <param name="state"></param>
        void Scan(object state);

    }
}
