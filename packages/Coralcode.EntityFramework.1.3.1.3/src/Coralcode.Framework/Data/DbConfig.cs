using Coralcode.Framework.Security;
using System;

namespace Coralcode.Framework.Data
{
    public class DbConfig
    {
        /// <summary>
        /// 编号
        /// </summary>
        public short Number { get; set; }

        /// <summary>
        /// 静态分库因子
        /// 垂直分库因子
        /// </summary>
        public string StaticCoden { get; set; }

        /// <summary>
        /// 动态分库因子
        /// 水平分库因子
        /// </summary>
        public string DynamicCoden { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string NameOrConnectionString { get; set; }

        /// <summary>
        /// 只写数据库连接字符串
        /// </summary>
        public string WriteConnectionString { get; set; }

        /// <summary>
        /// 只读数据库连接字符串
        /// </summary>
        public string ReadConnectionString { get; set; }

        /// <summary>
        /// 是否自动迁移数据
        /// </summary>
        public bool AutoMerageDataBase { get; set; }

        /// <summary>
        /// 是否未加密
        /// 默认是false;所以默认是加密的
        /// </summary>
        public bool UnEncoded { get; set; }

        /// <summary>
        /// 执行超时时间
        /// </summary>
        public int CommandTimeout { get; set; }

        public void PreAnalysis()
        {
            if (!UnEncoded && !string.IsNullOrEmpty(NameOrConnectionString))
                NameOrConnectionString = CoralSecurity.DesDecrypt(NameOrConnectionString);
            if (!UnEncoded && !string.IsNullOrEmpty(ReadConnectionString))
                ReadConnectionString = CoralSecurity.DesDecrypt(ReadConnectionString);
            if (!UnEncoded && !string.IsNullOrEmpty(WriteConnectionString))
                WriteConnectionString = CoralSecurity.DesDecrypt(WriteConnectionString);
            if (CommandTimeout == 0)
                CommandTimeout = 30;
        }

        public string GetIdentity()
        {
            return StaticCoden + NameOrConnectionString;
        }

    }
}
