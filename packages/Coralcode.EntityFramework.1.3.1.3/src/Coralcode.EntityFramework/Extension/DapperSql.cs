using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.EntityFramework.Extension
{
    public class DapperSql : ISql
    {
        private IDbConnection _connection;

        public DapperSql(IDbConnection connection)
        {
            _connection = connection;
        }

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
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, object parameter)
        {
            return _connection.Query<TEntity>(sqlQuery, parameter);
        }

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
        public int ExecuteCommand(string sqlCommand, object parameter)
        {
            return _connection.Execute(sqlCommand, parameter);
        }

        /// <summary>
        /// 根据存储过程获取DataTable
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sqlQuery, object parameter)
        {
            using (var reader = _connection.ExecuteReader(sqlQuery, parameter))
            {
                return DataReaderToDataTable(reader);
            }
        }
        private DataTable DataReaderToDataTable(IDataReader reader)
        {
            DataTable tb = new DataTable();
            DataColumn col;
            DataRow row;
            int i;

            for (i = 0; i < reader.FieldCount; i++)
            {
                col = new DataColumn();
                col.ColumnName = reader.GetName(i);
                col.DataType = reader.GetFieldType(i);
                tb.Columns.Add(col);
            }

            while (reader.Read())
            {
                row = tb.NewRow();
                for (i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader[i];
                }
                tb.Rows.Add(row);
            }
            return tb;
        }

        /// <summary>
        /// 根据存储过程获取DataTable
        /// 如果是分库，请在参数中提供IBranchRouter参数
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int GetCount(string sqlQuery, object parameter)
        {
            return _connection.ExecuteScalar<int>(sqlQuery, parameter);
        }
    }
}
