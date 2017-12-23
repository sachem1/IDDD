//===================================================================================

using System.Collections.Generic;

namespace Coralcode.Framework.Validator
{
    /// <summary>
    /// 实体验证
    /// </summary>
    public interface IEntityValidator
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsValid<TEntity>(TEntity item)
            where TEntity : class;

        /// <summary>
        /// 获取验证信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<string> GetInvalidMessages<TEntity>(TEntity item)
            where TEntity : class;
    }
}
