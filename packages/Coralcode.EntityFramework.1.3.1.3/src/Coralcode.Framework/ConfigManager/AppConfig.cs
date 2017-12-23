using System;
using System.IO;

namespace Coralcode.Framework.ConfigManager
{
    /// <summary>
    ///     配置文件类
    /// </summary>
    /// <remarks>
    ///     [2012-03-11]
    /// </remarks>
    public static class AppConfig
    {
        private static readonly AppSet appSettings = new AppSet();
        private static readonly DbConnectionSet connectionStrings = new DbConnectionSet();
        private static readonly DllSet dllConfigs = new DllSet();

        /// <summary>
        ///     获取AppSettings下的节点值
        /// </summary>
        public static AppSet AppSettings
        {
            get { return appSettings; }
        }

        /// <summary>
        ///     获取ConnectionStrings下的节点值
        /// </summary>
        public static DbConnectionSet ConnectionStrings
        {
            get { return connectionStrings; }
        }

        /// <summary>
        ///     获取DLL配置文件
        /// </summary>
        public static DllSet DllConfigs
        {
            get { return dllConfigs; }
        }

        /// <summary>
        ///     获取DllConfig的相对路径
        /// </summary>
        /// <param name="dllConfigName"></param>
        /// <returns></returns>
        private static string GetRelativePath(string dllConfigName)
        {
            var path = string.Format("~/Configs/{0}/{1}.config", AppSettings["EnvironmentParam"], dllConfigName);
            if (!string.IsNullOrEmpty(AppSettings["EnvironmentParam"]) && File.Exists(path))
                return string.Format("~/Configs/{0}.config", dllConfigName);
            return path;
        }

        /// <summary>
        ///     获取DllConfig的绝对路径
        /// </summary>
        /// <param name="dllConfigName"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(string dllConfigName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", string.Format("{0}.config", dllConfigName));
            if (File.Exists(path))
                return path;
            var enviromentParam = AppSettings["EnvironmentParam"];
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", enviromentParam, string.Format("{0}.config", dllConfigName));
            if (string.IsNullOrEmpty(enviromentParam)&&!File.Exists(path))
                throw new FileNotFoundException(path+"未找到");
            if (File.Exists(path))
                return path;
            throw new FileNotFoundException("路径:"+ path, dllConfigName);
        }
        /// <summary>
        ///     获取DllConfig的绝对路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileByAbsolutePath(string fileName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", fileName);
            if (File.Exists(path))
                return path;
            var enviromentParam = AppSettings["EnvironmentParam"];
            if (string.IsNullOrEmpty(enviromentParam))
                throw new FileNotFoundException("EnvironmentParam 未找到");
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", enviromentParam, fileName);
            if (File.Exists(path))
                return path;
            throw new FileNotFoundException("路径:" + path, fileName);
        }

        /// <summary>
        ///     检查DllConfig是否存在
        /// </summary>
        /// <param name="dllConfigName"></param>
        /// <returns></returns>
        public static bool IsExists(string dllConfigName)
        {
            var path = GetAbsolutePath(dllConfigName);
            return File.Exists(path);
        }
    }
}