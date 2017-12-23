using System.Data;

namespace Coralcode.Framework.Data.Extension
{
    public interface IQuery
    {
        /// <summary>
        /// 根据存储过程获取DataTable
        /// 如果执行sql的话，使用sqlparameter参数
        /// 如果是分库，请在参数中提供IBranchRouter参数
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataTable GetDataTable<TEntity>(string queryString, params object[] parameters);

        /// <summary>
        /// 根据存储过程获取DataTable
        /// 如果执行sql的话，使用sqlparameter参数
        /// 如果是分库，请在参数中提供IBranchRouter参数
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int GetCount<TEntity>(string queryString, params object[] parameters);
        
    }
}
