using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Data.Core;

namespace Coralcode.Framework.Data.SofeDelete
{
    /// <summary>
    /// 软删除实体
    /// </summary>
    public abstract class SoftDeleteEntity :Entity, ISoftDelete
    {
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
