using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Reflection;

namespace Coralcode.Framework.Exceptions
{
    public class CoralErrorCode
    {

        internal static Dictionary<int, string> ExceptionTable = new Dictionary<int, string>();

        static CoralErrorCode()
        {
            var errorCodeTypes = MetaDataManager.Type.Find(item => item.IsSubclassOf(typeof(CoralErrorCode)) || item == typeof(CoralErrorCode));
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

            foreach (var errorCodeType in errorCodeTypes)
            {
                var fields = errorCodeType.GetFields(flag).ToList();
                var instance = Activator.CreateInstance(errorCodeType);

                foreach (var fieldInfo in fields)
                {
                    var value = (int)fieldInfo.GetValue(instance);
                    var descAtt = (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
                    if (ExceptionTable.ContainsKey(value))
                    {
                        throw new ArgumentException("指定的错误编码:"+value+"已存在",fieldInfo.Name);
                    }

                    ExceptionTable.Add(value, descAtt.Description);

                }
            }
        }

        /// <summary>
        /// 系统错误
        /// </summary>
        [Description("系统错误")]
        public int SystemError = 10001;


        /// <summary>
        /// 配置文件错误
        /// </summary>
        [Description("配置文件错误")]
        public int ConfigError = 10101;

        /// <summary>
        /// 模块错误
        /// </summary>
        [Description("模块错误")]
        public int ModuleError = 10102;

        /// <summary>
        /// 类型对应模块未找到
        /// </summary>
        [Description("类型对应模块未找到")]
        public int TypeNotInModule = 10103;

        /// <summary>
        /// 类型对应模块未找到
        /// </summary>
        [Description("模块已存在")]
        public int ModuleExisted = 10104;


        

        #region 服务级错误代码


        /// <summary>
        /// 非法请求
        /// </summary>
        [Description("非法请求")]
        public int IllegalRequest = 13001;

        /// <summary>
        /// 缺失参数
        /// </summary>
        [Description("缺失参数")]
        public int MissParameter = 13002;

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        public int ParamError = 13003;

        /// <summary>
        /// 请求长度超过限制
        /// </summary>
        [Description("请求长度超过限制")]
        public int RequestBodyLengthOverLimit = 13004;

        /// <summary>
        /// 记录已存在
        /// </summary>
        [Description("记录已存在")]
        public int ObjectAlreadyExists = 13005;

        /// <summary>
        /// 未能找到请求头信息
        /// </summary>
        [Description("未能找到请求头信息")]
        public int HeandersMissing = 13006;

        /// <summary>
        /// 请求头信息错误
        /// </summary>
        [Description("请求头信息错误")]
        public int HeandersError = 13007;


        /// <summary>
        /// 应用的接口访问权限受限
        /// </summary>
        [Description("应用的接口访问权限受限")]
        public int InsufficientAppPermissions = 13008;


        /// <summary>
        /// 接口不存在
        /// </summary>
        [Description("接口不存在")]
        public int RequestApiNotFound = 13009;

        /// <summary>
        /// 用户请求频次超过上限
        /// </summary>
        [Description("用户请求频次超过上限")]
        public int UserRequestsOutOfRateLimit = 13010;


        /// <summary>
        /// 远程服务错误
        /// <remarks>如：WebUtil在请求某个接口时发生错误抛出此错误</remarks>
        /// </summary>
        [Description("远程服务错误")]
        public int RemoteServiceError = 13011;


        /// <summary>
        /// 远程服务请求失败
        /// <remarks>如：WebUtil在请求某个接口时失败抛出此错误</remarks>
        /// </summary>
        [Description("远程服务请求失败")]
        public int RemoteServiceRequestFail = 13012;

        /// <summary>
        /// 不支持的MediaType
        /// <remarks>如需要文件流数据，却传输的文本信息抛出此错误</remarks>
        /// </summary>
        [Description("不支持的MediaType")]
        public int UnsupportMediatype =13013;
        #endregion

    }

}
