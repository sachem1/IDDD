using System.ComponentModel;
using Coralcode.Framework.Exceptions;

namespace Coralcode.EntityFramework.Extension
{
    /// <summary>
    /// 前缀15
    /// </summary>
    public class DbErrorCode : CoralErrorCode
    {
        /// <summary>
        /// 无效数据库配置
        /// </summary>
        [Description("无效数据库配置")]
        public int InvalideDbConfig = 15001;

        /// <summary>
        /// 无效的数据库代码
        /// </summary>
        [Description("无效的数据库代码")]
        public int InvalideDbCoden = 15002;
    }
}
