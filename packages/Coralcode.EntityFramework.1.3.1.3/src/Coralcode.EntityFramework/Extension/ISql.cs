using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.EntityFramework.Extension
{
    public interface ISql
    {

        /// <summary>
        /// 执行查询获取实体
        /// </summary>
        /// <typeparam name="TEntity">Entity type to map query results</typeparam>
        /// <param name="sqlQuery">
        /// Dialect Query 
        /// <example>
        /// SELECT idCustomer,Name FROM dbo.[Customers] WHERE idCustomer > {0}
        /// </example>
        /// </param>
        /// <param name="parameter">A vector of parameters values</param>
        /// <returns>
        /// Enumerable results 
        /// </returns>
        IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, object parameter);

        /// <summary>
        /// Execute arbitrary command into underliying persistence store
        /// </summary>
        /// <param name="sqlCommand">
        /// Command to execute
        /// <example>
        /// SELECT idCustomer,Name FROM dbo.[Customers] WHERE idCustomer > {0}
        /// </example>
        ///</param>
        /// <param name="parameter">A vector of parameters values</param>
        /// <returns>The number of affected records</returns>
        int ExecuteCommand(string sqlCommand, object parameter);


        /// <summary>
        /// 根据存储过程获取DataTable
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        DataTable GetDataTable(string sqlQuery, object parameter);

        /// <summary>
        /// 根据存储过程获取DataTable
        /// 如果执行sql的话，使用匿名对象参数
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int GetCount(string sqlQuery, object parameter);
    }
}
