using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Coralcode.Framework.Security;

namespace Coralcode.Framework.Web
{
    /// <summary>
    /// <para>　</para>
    /// 　常用工具类——COOKIES操作类
    /// <para>　-------------------------------------------------------------------</para>
    /// <para>　WriteCookie：创建COOKIE对象并赋Value值或值集合 [+4重载]</para>
    /// <para>　GetCookie：读取Cookie某个对象的Value值，返回Value值，如果对象本就不存在，则返回null</para>
    /// <para>　DelCookie：删除COOKIE对象</para>
    /// </summary>
    public class CoralCookies
    {

        #region 创建COOKIE对象并赋Value值
        /// <summary>
        /// 创建COOKIE对象并赋Value值
        /// </summary>
        /// <param name="cookiesName">COOKIE对象名</param>
        /// <param name="IExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>  
        /// <param name="cookiesValue">COOKIE对象Value值</param>
        public static void WriteCookies(string cookiesName, string cookiesValue, int IExpires = 1)
        {
            HttpCookie objCookie = new HttpCookie(cookiesName.Trim());
            objCookie.Value = CoralSecurity.DesEncrypt(cookiesValue.Trim());    //加密存储
            if (IExpires > 0)
            {
                if (IExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddMinutes(IExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>
        /// 创建COOKIE对象并赋Value值
        /// </summary>
        /// <param name="cookiesName">COOKIE对象名</param>
        /// <param name="IExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>  
        /// <param name="cookiesValue">COOKIE对象Value值</param>
        /// <param name="CookiesDomain">作用域</param>
        public static void WriteCookies(string cookiesName, string cookiesValue, string CookiesDomain, int IExpires = 1)
        {
            HttpCookie objCookie = new HttpCookie(cookiesName.Trim());
            objCookie.Value = CoralSecurity.DesEncrypt(cookiesValue.Trim());    //加密存储
            objCookie.Domain = CookiesDomain.Trim();
            if (IExpires > 0)
            {
                if (IExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddMinutes(IExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>   
        /// 创建COOKIE对象并赋多个KEY键值   
        /// 设键/值如下：   
        /// NameValueCollection myCol = new NameValueCollection();   
        /// myCol.Add("red", "rojo");   
        /// myCol.Add("green", "verde");   
        /// myCol.Add("blue", "azul");   
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”   
        /// </summary>   
        /// <param name="cookiesName">COOKIE对象名</param>   
        /// <param name="expires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>   
        /// <param name="cookiesKeyValueCollection">键/值对集合</param> 
        public static void WriteCookies(string cookiesName, int expires, NameValueCollection cookiesKeyValueCollection)
        {
            HttpCookie objCookie = new HttpCookie(cookiesName.Trim());
            foreach (String key in cookiesKeyValueCollection.AllKeys)
            {
                objCookie[key] = CoralSecurity.DesEncrypt(cookiesKeyValueCollection[key].Trim());
            }
            if (expires > 0)
            {
                if (expires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(expires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>   
        /// 创建COOKIE对象并赋多个KEY键值   
        /// 设键/值如下：   
        /// NameValueCollection myCol = new NameValueCollection();   
        /// myCol.Add("red", "rojo");   
        /// myCol.Add("green", "verde");   
        /// myCol.Add("blue", "azul");   
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”   
        /// </summary>   
        /// <param name="cookiesName">COOKIE对象名</param>   
        /// <param name="expires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>   
        /// <param name="cookiesKeyValueCollection">键/值对集合</param> 
        /// <param name="cookiesDomain">作用域</param>
        public static void WriteCookies(string cookiesName, int expires, NameValueCollection cookiesKeyValueCollection, string cookiesDomain)
        {
            HttpCookie objCookie = new HttpCookie(cookiesName.Trim());
            foreach (String key in cookiesKeyValueCollection.AllKeys)
            {
                objCookie[key] = CoralSecurity.DesEncrypt(cookiesKeyValueCollection[key].Trim());
            }
            objCookie.Domain = cookiesDomain.Trim();
            if (expires > 0)
            {
                if (expires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(expires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        #endregion

        #region 读取Cookie某个对象的Value值，返回Value值，如果对象本就不存在，则返回null


        /// <summary>
        /// 判断Cookie是否存在
        /// </summary>
        /// <param name="cookiesName">Cookie对象名称</param>
        /// <returns></returns>
        public static bool ExistCookies(string cookiesName)
        {
            return HttpContext.Current.Request.Cookies[cookiesName] != null;

        }

        /// <summary>
        /// 判断Cookie的某个键是否存在
        /// </summary>
        /// <param name="cookiesName">Cookie对象名称</param>
        /// <param name="keyName">键值</param>
        /// <returns></returns>
        public static bool ExistCookies(string cookiesName, string keyName)
        {




            if (HttpContext.Current.Request.Cookies[cookiesName] == null)
            {
                return false;
            }
            else
            {
                var strObjValue = CoralSecurity.DesDecrypt(HttpContext.Current.Request.Cookies[cookiesName].Value);
                var strKeyName2 = keyName + "=";
                return strObjValue.IndexOf(strKeyName2, StringComparison.Ordinal) != -1;
            }
        }

        /// <summary>
        /// 读取Cookie某个对象的Value值，返回Value值，如果对象本就不存在，则返回null
        /// </summary>
        /// <param name="cookiesName">Cookie对象名称</param>
        /// <returns>返回对象的Value值，返回Value值，如果对象本就不存在，则返回null</returns>
        public static string GetCookies(string cookiesName)
        {
            if (HttpContext.Current.Request.Cookies[cookiesName] == null)
            {
                return null;
            }
            else
            {
                return CoralSecurity.DesDecrypt(HttpContext.Current.Request.Cookies[cookiesName].Value);
            }
        }

        /// <summary>
        /// 读取Cookie某个对象的Value值，返回Value值，如果对象本就不存在，则返回null
        /// </summary>
        /// <param name="cookiesName">Cookie对象名称</param>
        /// <param name="keyName">键值</param>
        /// <returns>返回对象的Value值，返回Value值，如果对象本就不存在，则返回null</returns>
        public static string GetCookies(string cookiesName, string keyName)
        {
            if (HttpContext.Current.Request.Cookies[cookiesName] == null)
            {
                return null;
            }
            else
            {
                string strObjValue = CoralSecurity.DesDecrypt(HttpContext.Current.Request.Cookies[cookiesName].Value);
                string strKeyName2 = keyName + "=";
                if (strObjValue.IndexOf(strKeyName2) == -1)
                {
                    return null;
                }
                else
                {
                    return CoralSecurity.DesDecrypt(HttpContext.Current.Request.Cookies[cookiesName][keyName]);
                }
            }
        }
        #endregion

        #region 删除COOKIE对象
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="cookiesName">Cookie对象名称</param>
        public static void DelCookie(string cookiesName)
        {
            HttpCookie objCookie = new HttpCookie(cookiesName.Trim());
            objCookie.Expires = DateTime.Now.AddYears(-5);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>
        /// 清空COOKIE对象
        /// </summary>
        public static void Clear()
        {
            List<string> keys = HttpContext.Current.Request.Cookies.Cast<string>().ToList();

            keys.ForEach(DelCookie);
        }

        #endregion
    }
}
