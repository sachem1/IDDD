using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Coralcode.Framework.Utils
{
    public class StringUtil
    {
        /// <summary>
        /// 格式化url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Format(string url, object obj)
        {
            Dictionary<string, string> urlParams = new Dictionary<string, string>();

            var objType = obj.GetType();
            var properties = objType.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                string propertyName = property.Name;
                var value = objType.GetProperty(propertyName, property.PropertyType).GetValue(obj);
                if (value == null)
                {
                    urlParams.Add(propertyName, "null");
                    continue;
                }
                urlParams.Add(propertyName, value.ToString());

            }

            return Format(url, urlParams);
        }

        /// <summary>
        /// 格式化url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlParams"></param>
        /// <returns></returns>

        public static string Format(string url, Dictionary<string, string> urlParams)
        {
            List<string> urlStrs = new List<string>();

            foreach (var urlParam in urlParams)
            {
                urlStrs.Add(string.Format("{0}={1}", urlParam.Key, urlParam.Value));
            }

            string urlPrefix = "http://";

            return string.Format("{0}{1}?{2}",
                url.StartsWith(urlPrefix) ? "" : urlPrefix,
                url,
                string.Join("&", urlStrs));

        }


        public static string FormatKey(params string[] keys)
        {
            return string.Join("-", keys);
        }


        /// <summary>
        /// 获取第一次在两个文本之间的部分
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static string GetFirstText(string source, string startString, string endString)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(startString) || string.IsNullOrEmpty(endString))
                return string.Empty;
            int num1 = source.IndexOf(startString, StringComparison.CurrentCultureIgnoreCase);
            if (num1 == -1)
                return string.Empty;
            int startIndex = num1 + startString.Length;
            int num2 = source.IndexOf(endString, startIndex, StringComparison.CurrentCultureIgnoreCase);
            if (num2 == -1)
                return string.Empty;
            else
                return source.Substring(startIndex, num2 - startIndex);
        }

        /// <summary>
        /// 获取最后一次在两个文本之间的部分
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static string GetLastText(string htmlText, string startString, string endString)
        {
            if (string.IsNullOrEmpty(htmlText) || string.IsNullOrEmpty(startString) || string.IsNullOrEmpty(endString))
                return string.Empty;
            int num1 = htmlText.LastIndexOf(endString, StringComparison.CurrentCultureIgnoreCase);
            if (num1 == -1)
                return string.Empty;
            int endIndex = num1;
            var strBefore = htmlText.Substring(0, num1);
            int num2 = strBefore.LastIndexOf(startString, StringComparison.CurrentCultureIgnoreCase);
            if (num2 == -1)
                return string.Empty;
            else
                return strBefore.Substring(num2, endIndex - num2);
        }

        /// <summary>   
        /// 清除HTML标记   
        /// </summary>   
        /// <param name="htmlstring">包括HTML的源码</param>
        /// <returns>已经去除后的文字</returns>   
        public static string NoHtml(string htmlstring)
        {
            //删除脚本  
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            return htmlstring.Replace("<", "").Replace(">", "").Replace("\r\n", "");
        }
    }
}