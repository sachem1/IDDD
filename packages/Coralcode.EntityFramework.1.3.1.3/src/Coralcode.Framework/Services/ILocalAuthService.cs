using System.ComponentModel;
using Coralcode.Framework.GenericsFactory;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.Services
{
    public interface ILocalAuthService:IStrategy
    {
        LoginState Login(string account, string pwd, out UserModel model);

        /// <summary>
        /// 查询用户数据
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        UserModel FindUser(string account);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="account"></param>
        void Logout(string account);
    }


    public enum LoginState
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("未知错误")]
        UnknowException,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success,
        /// <summary>
        /// 用户名未找到
        /// </summary>
        [Description("用户名未找到")]
        UserNotFind,
        /// <summary>
        /// 密码错误
        /// </summary>
        [Description("密码错误")]
        PasswordError,
        /// <summary>
        /// 用户不可用
        /// </summary>
        [Description("用户不可用")]
        UserUnavailable,
        /// <summary>
        /// 用户已过期
        /// </summary>
        [Description("用户已过期")]
        UserExpired
    }
}
