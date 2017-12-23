using System;

namespace Coralcode.Framework.Data
{
    [Flags]
    public enum DbMode
    {
        /// <summary>
        /// 只读
        /// </summary>
        Write = 1,
        /// <summary>
        /// 只写
        /// </summary>
        Read = 2
    }


    public static class DbModelExtensions
    {

        /// <summary>
        /// 只读
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool IsReadOnly(this DbMode mode)
        {
            return mode.HasFlag(DbMode.Read) && !mode.HasFlag(DbMode.Write);
        }

        /// <summary>
        /// 可以读,对写入无限制
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool CanRead(this DbMode mode)
        {
            return mode.HasFlag(DbMode.Read);
        }

        /// <summary>
        /// 只写入
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>

        public static bool IsWriteOnly(this DbMode mode)
        {
            return mode.HasFlag(DbMode.Write) && !mode.HasFlag(DbMode.Read);
        }

        /// <summary>
        /// 可以写，对读无限制
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool CanWrite(this DbMode mode)
        {
            return mode.HasFlag(DbMode.Write);
        }

        /// <summary>
        /// 可以同事读写
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool IsReadAndWrite(this DbMode mode)
        {
            return mode.HasFlag(DbMode.Read) && mode.HasFlag(DbMode.Write);
        }
    }
}