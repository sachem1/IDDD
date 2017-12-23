using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Resources;

namespace Coralcode.Framework.Utils
{
    public static class SmsUtil
    {
        private static readonly List<string> InvalidWords = InValidWords.SmsInvalidWords.Split('/').Where(item=>!string.IsNullOrWhiteSpace(item.Trim())).ToList(); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="content"></param>
        /// <returns>
        /// -1	没有该用户账户
        ///-2	接口密钥不正确 [查看密钥]不是账户登陆密码
        ///-21	MD5接口密钥加密不正确
        ///-3	短信数量不足
        ///-11	该用户被禁用
        ///-14	短信内容出现非法字符
        ///-4	手机号格式不正确
        ///-41	手机号码为空
        ///-42	短信内容为空
        ///-51	短信签名格式不正确接口签名格式为：【签名内容】
        ///-6	IP限制
        ///大于0	短信发送数量
        /// </returns>
        public static string Send(string mobileNumber, string content)
        {
            
            if(InvalidWords.Any(item=>content.Contains(item)))
                throw new ArgumentException("短信内容包含非法字符");

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(AppConfig.DllConfigs.Current["Sms"]["Host"])
            };
            var resp = httpClient.GetAsync(string.Format(AppConfig.DllConfigs.Current["Sms"]["Url"], mobileNumber, content)).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="content"></param>
        /// <returns>
        /// -1请求异常
        /// 1发送成功
        /// 
        /// </returns>
        public static string SendByIpyy(string mobileNumber, string content)
        {
            //if (InvalidWords.Any(item => content.Contains(item)))
            //    throw new ArgumentException("短信内容包含非法字符");
            var myEncoding = Encoding.GetEncoding("UTF-8");
            var req = WebRequest.Create(AppConfig.DllConfigs.Current["Sms"]["IpyyHost"]);
            var param = string.Format(AppConfig.DllConfigs.Current["Sms"]["IpyyUrl"],
                HttpUtility.UrlEncode(mobileNumber, myEncoding), HttpUtility.UrlEncode("【中估联行】" + content, myEncoding));
            var postBytes = Encoding.ASCII.GetBytes(param);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            req.ContentLength = postBytes.Length;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);
            }

            var xmlDoc = new System.Xml.XmlDocument();
            using (WebResponse wr = req.GetResponse())
            {
                var sr = new StreamReader(wr.GetResponseStream(), System.Text.Encoding.UTF8);
                xmlDoc.Load(sr);
            }

            var xmlNode = xmlDoc.GetElementsByTagName("message").Item(0);
            if(xmlNode==null) return "供应商接口返回异常";

            var message = xmlNode.InnerText.ToString();
            return message == "操作成功" ? "操作成功" : message;
        }

    }
}
