using Coralcode.Framework.Log;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coralcode.EntityFramework.Extension
{
    /// <summary>
    /// 使用示例
    ///  DbInterception.Add(new EFIntercepterLogging());
    /// </summary>
    public class EFIntercepterLogging : DbCommandInterceptor
    {
        [ThreadStatic]
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly ILogger _logger = LoggerFactory.GetLogger("EntityFrameSqlTrace");

        const string errorFormat = "SqlException:{1} rn --> Error executing command:{0}";
        const string timeTraceFormat = "接口：{0},方法：{1},参数：{2},耗时：{3}";


        public override void ScalarExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            _stopwatch.Restart();
            base.ScalarExecuting(command, interceptionContext);
        }
        public override void ScalarExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            _stopwatch.Stop();
            OutputLog(command, interceptionContext, "ScalarExecuted");

            base.ScalarExecuted(command, interceptionContext);
        }
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }
        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            _stopwatch.Stop();
            OutputLog(command, interceptionContext, "NonQueryExecuted");

            base.NonQueryExecuted(command, interceptionContext);
        }
        public override void ReaderExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            base.ReaderExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }
        public override void ReaderExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            _stopwatch.Stop();
            OutputLog(command, interceptionContext, "ReaderExecuted");
            base.ReaderExecuted(command, interceptionContext);
        }

        private void OutputLog<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext,string tag)
        {
            if (interceptionContext.Exception != null)
            {
                _logger.Error(errorFormat, FormatSql(command), interceptionContext.Exception.ToString());
            }
            else
            {
                _logger.Info(timeTraceFormat, tag, FormatSql(command), "", _stopwatch.ElapsedMilliseconds);
            }
        }

        private string GetParamValue(DbCommand command)
        {
            var param = string.Empty;
            foreach (DbParameter commandParameter in command.Parameters)
            {
                param += commandParameter.ParameterName + "-" + Convert.ToString(commandParameter.Value) + "/";
            }
            return param;
        }

        /// <summary>
        /// 格式化sql
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string FormatSql(DbCommand cmd)
        {
            string sql = cmd.CommandText;
            foreach (DbParameter cmdParam in cmd.Parameters)
            {
                sql = sql.Replace($"@{cmdParam.ParameterName}", $"'{Convert.ToString(cmdParam.Value)}'");
            }
            return sql;
        }
    }
}
