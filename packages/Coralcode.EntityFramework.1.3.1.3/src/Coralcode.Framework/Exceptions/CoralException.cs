using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;
using Coralcode.Framework.Reflection;
using Newtonsoft.Json;

namespace Coralcode.Framework.Exceptions
{

    /// <summary>
    /// 业务异常
    /// </summary>
    public class CoralException : Exception
    {

        internal protected CoralException(string message) : base(message)
        {
            HResult = 10001;
        }
        internal protected CoralException(string message, Exception innerException) : base(message, innerException)
        {
            HResult = 10001;
        }


        public static CoralException ThrowException(Expression<Func<CoralErrorCode, int>> errorProperty,
                string message = null,
                Exception innerException = null,
                object param = null)
        {
            return ThrowException<CoralException, CoralErrorCode>(errorProperty, message, innerException, param);
        }


        public static CoralException ThrowException<TErrorCode>(Expression<Func<TErrorCode, int>> errorProperty,
                string message = null,
                Exception innerException = null,
                object param = null
                )
              where TErrorCode : CoralErrorCode
        {
            return ThrowException<CoralException, TErrorCode>(errorProperty, message, innerException, param);
        }

        public static TException ThrowException<TException, TErrorCode>(
            Expression<Func<TErrorCode, int>> errorProperty,
            string message = null,
            Exception innerException = null,
                object param = null
            )
            where TException : CoralException
            where TErrorCode : CoralErrorCode
        {

            var errorCode = Activator.CreateInstance<TErrorCode>();

            var codeValue = errorProperty.Compile()(errorCode);

            if (message == null)
            {
                //这里根据错误表查询错误对应的描述信息，赋值message里面
                //如果对应的errorcode没有标记则直接取errorcode
                if (!CoralErrorCode.ExceptionTable.TryGetValue(codeValue, out message))
                    message = codeValue.ToString();
            }
            TException exception;
            if (innerException == null)
            {
                exception = (TException)Activator.CreateInstance(typeof(TException),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                   null,
                   new object[] { message }, CultureInfo.DefaultThreadCurrentCulture);

            }
            else
            {
                exception = (TException)Activator.CreateInstance(
                    typeof(TException),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { message, innerException }, CultureInfo.DefaultThreadCurrentCulture);

            }
            exception.HResult = codeValue;
            if (param != null)
                exception.Data.Add("Param", param);
            return exception;
        }

       
    }



    #region 不要的错误代码表示
    ///// <summary>
    ///// 错误代码
    ///// </summary>
    //public enum EnumErrorCode
    //{

    //    #region 系统级错误代码

    //    /// <summary>
    //    /// 系统错误
    //    /// </summary>
    //    [JsonProperty("System error")]
    //    [Description("系统错误")]
    //    SystemError = 10001,

    //    /// <summary>
    //    /// 服务暂停
    //    /// </summary>
    //    [JsonProperty("Service unavailable")]
    //    [Description("服务暂停")]
    //    ServiceUnavailable = 10002,

    //    /// <summary>
    //    /// 远程服务错误
    //    /// </summary>
    //    [JsonProperty("Remote service error")]
    //    [Description("远程服务错误")]
    //    RemoteServiceError = 10003,

    //    /// <summary>
    //    /// IP限制不能请求该资源
    //    /// </summary>
    //    [JsonProperty("IP limit")]
    //    [Description("IP限制不能请求该资源")]
    //    IPLimit = 10004,

    //    /// <summary>
    //    /// 该资源需要appToken拥有授权
    //    /// </summary>
    //    [JsonProperty("Permission denied, need a high level appkey")]
    //    [Description("该资源需要appToken拥有授权")]
    //    PermissionDeniedNeedAHighLevelAppToken = 10005,

    //    /// <summary>
    //    /// 缺少source (appToken) 参数
    //    /// </summary>
    //    [JsonProperty("Source paramter (appkey) is missing")]
    //    [Description("缺少source (appToken) 参数")]
    //    SourceParamterAppTokenIsMissing = 10006,

    //    /// <summary>
    //    /// 不支持的MediaType (%s)
    //    /// </summary>
    //    [JsonProperty("Unsupport mediatype (%s)")]
    //    [Description("不支持的MediaType (%s)")]
    //    UnsupportMediatype = 10007,

    //    /// <summary>
    //    /// 参数错误，请参考API文档
    //    /// </summary>
    //    [Description("参数错误，请参考API文档")]
    //    ParamErrorSeeDocForMoreInfo = 10008,

    //    /// <summary>
    //    /// 任务过多，系统繁忙
    //    /// </summary>
    //    [Description("任务过多，系统繁忙")]
    //    TooManyPendingTasksSystemIsBusy = 10009,

    //    /// <summary>
    //    /// 任务超时
    //    /// </summary>
    //    [Description("任务超时")]
    //    JobExpired = 10010,

    //    /// <summary>
    //    /// RPC错误
    //    /// </summary>
    //    [Description("RPC错误")]
    //    RPCError = 10011,

    //    /// <summary>
    //    /// 非法请求
    //    /// </summary>
    //    [Description("非法请求")]
    //    IllegalRequest = 10012,

    //    /// <summary>
    //    /// 不合法的VISS用户
    //    /// </summary>
    //    [Description("不合法的VISS用户")]
    //    InvalidVISSUser = 10013,

    //    /// <summary>
    //    /// 应用的接口访问权限受限
    //    /// </summary>
    //    [Description("应用的接口访问权限受限")]
    //    InsufficientAppPermissions = 10014,

    //    /// <summary>
    //    /// 缺失必选参数 (%s)，请参考API文档
    //    /// </summary>
    //    [Description("缺失必选参数 (%s)，请参考API文档")]
    //    MissRequiredParameterSeeDocForMoreInfo = 10016,

    //    /// <summary>
    //    /// 参数值非法，需为 (%s)，实际为 (%s)，请参考API文档
    //    /// </summary>
    //    [Description("参数值非法，需为 (%s)，实际为 (%s)，请参考API文档")]
    //    ParameterValueInvalidExpectButGetSeeDocForMoreInfo = 10017,

    //    /// <summary>
    //    /// 请求长度超过限制
    //    /// </summary>
    //    [Description("请求长度超过限制")]
    //    RequestBodyLengthOverLimit = 10018,

    //    /// <summary>
    //    /// 接口不存在
    //    /// </summary>
    //    [Description("接口不存在")]
    //    RequestApiNotFound = 10020,

    //    /// <summary>
    //    /// 请求的HTTP METHOD不支持，请检查是否选择了正确的POST/GET方式
    //    /// </summary>
    //    [Description("请求的HTTP METHOD不支持，请检查是否选择了正确的POST/GET方式")]
    //    HTTPMethodIsNotSuportedForThisRequest = 10021,

    //    /// <summary>
    //    /// IP请求频次超过上限
    //    /// </summary>
    //    [Description("IP请求频次超过上限")]
    //    IPRequestsOutOfRateLimit = 10022,

    //    /// <summary>
    //    /// 用户请求频次超过上限
    //    /// </summary>
    //    [Description("用户请求频次超过上限")]
    //    UserRequestsOutOfRateLimit = 10023,

    //    /// <summary>
    //    /// 用户请求特殊接口 (%s) 频次超过上限
    //    /// </summary>
    //    [Description("用户请求特殊接口 (%s) 频次超过上限")]
    //    UserRequestsForOutOfRateLimit = 10024,


    //    /// <summary>
    //    /// 缺少source (version) 参数
    //    /// </summary>
    //    [JsonProperty("Source paramter (version) is missing")]
    //    [Description("缺少source (version) 参数")]
    //    SourceParamterVersionIsMissing = 10025,

    //    /// <summary>
    //    /// 缺失设备ID参数
    //    /// </summary>
    //    [Description("缺失设备ID参数")]
    //    MissDeviceIdParameter = 10026,

    //    /// <summary>
    //    /// Basic参数无效
    //    /// </summary>
    //    [Description("Basic参数无效")]
    //    InvalidBasic = 10027,

    //    /// <summary>
    //    /// 缺少source (Basic) 参数
    //    /// </summary>
    //    [Description("缺少source (Basic) 参数")]
    //    SourceParamterBasicIsMissing = 10028,

    //    /// <summary>
    //    /// AppToken过期
    //    /// </summary>
    //    [Description("AppToken过期")]
    //    ExpiredToken = 21327,




    //    #endregion

    //    #region 服务级错误代码

    //    /// <summary>
    //    /// IDs参数为空
    //    /// </summary>
    //    [JsonProperty("IDs is null")]
    //    [Description("IDs参数为空")]
    //    IDsIsNull = 20001,

    //    /// <summary>
    //    /// 附件上传太多，请确认不要超过1个
    //    /// </summary>
    //    [Description("附件上传太多，请确认不要超过1个")]
    //    AttachmentTooMany = 20002,

    //    /// <summary>
    //    /// 附件数据已修改或与已有附件信息不匹配
    //    /// </summary>
    //    [Description("附件数据已修改或与已有附件信息不匹配")]
    //    AttachmentModified = 20003,

    //    /// <summary>
    //    /// 记录已存在
    //    /// </summary>
    //    [Description("记录已存在")]
    //    ObjectAlreadyExists = 20606,

    //    #endregion

    //    #region 架构级别错误
    //    /// <summary>
    //    /// 模块错误
    //    /// </summary>
    //    [Description("模块错误")]
    //    ModuleError = 30101,

    //    /// <summary>
    //    /// 类型对应模块未找到
    //    /// </summary>
    //    [Description("类型对应模块未找到")]
    //    TypeNotInModule = 30102,
    //    #endregion

    //}
    #endregion
}