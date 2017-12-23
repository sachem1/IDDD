using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Coralcode.Framework.Utils
{
    public static class HttpUtil
    {

        private static TimeSpan _timeout = new TimeSpan(0, 5, 0);
        public static void SetTimeOut(TimeSpan timeSpan)
        {
            _timeout = timeSpan;
        }

        public static void ResetDefaultTimeOut()
        {
            _timeout = new TimeSpan(0, 5, 0);
        }

        /// <summary>
        /// 默认5分钟
        /// </summary>
        /// <returns></returns>
        private static TimeSpan GetTimeOut()
        {
            return _timeout;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHtml(string host, string url)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(host),
                Timeout = _timeout

            };


            var resp = httpClient.GetAsync(url).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsStringAsync().Result;

        }

        #region 基于json的webapi请求

        /// <summary>
        /// Request Method:GET
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="headerValues">Request Headers</param>
        /// <param name="dict">URL?参数</param>
        /// <param name="token"></param>
        /// <returns>发送GET请求，接收返回值</returns>
        public static TResult Get<TResult>(string host, string url, Dictionary<string, string> headerValues = null, Dictionary<string, string> dict = null)
        {
            var client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            var  urlWithParam= AppendParamToUrl(url, dict);
            SetHttps(host);
            return Request<TResult>(client, urlWithParam, (innerClient, innerUrl) => innerClient.GetAsync(innerUrl));
        }
        /// <summary>
        /// Request Method:GET
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="headerValues">Request Headers</param>
        /// <param name="dict">URL?参数</param>
        /// <param name="token">取消</param>
        /// <returns>发送GET请求，接收返回值</returns>
        public static TResult Get<TResult>(string host, string url, CancellationToken token, Dictionary<string, string> headerValues = null, Dictionary<string, string> dict = null)
        {
            var client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            var urlWithParam = AppendParamToUrl(url, dict);
            SetHttps(host);
            return Request<TResult>(client, urlWithParam, token, (innerClient, innerUrl, innerToken) => innerClient.GetAsync(innerUrl, innerToken));
        }


        /// <summary>
        /// Request Method:GET
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="headerValues">Request Headers</param>
        /// <param name="dict">URL?参数</param>
        /// <returns>发送GET请求，接收返回值</returns>
        public static TResult Delete<TResult>(string host, string url, Dictionary<string, string> headerValues = null, Dictionary<string, string> dict = null)
        {
            var client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            var urlWithParam = AppendParamToUrl(url, dict);
            SetHttps(host);
            return Request<TResult>(client, urlWithParam, (innerClient, innerUrl) => innerClient.DeleteAsync(innerUrl));
        }


        /// <summary>
        /// Request Method:GET
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="token"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <param name="dict">URL?参数</param>
        /// <returns>发送GET请求，接收返回值</returns>
        public static TResult Delete<TResult>(string host, string url, CancellationToken token, Dictionary<string, string> headerValues = null, Dictionary<string, string> dict = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            var urlWithParam = AppendParamToUrl(url, dict);
            SetHttps(host);
            return Request<TResult>(client, urlWithParam, token, (innerClient, innerUrl, innerToken) => innerClient.DeleteAsync(innerUrl, innerToken));


        }

        /// <summary>
        /// Request Method:POST
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="param">对象</param>
        /// <param name="token"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Post<TParam, TResult>(string host, string url, TParam param, CancellationToken token, Dictionary<string, string> headerValues = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, token, (innerClient, innerUrl, innerContent, innerToken) => innerClient.PostAsync(innerUrl, innerContent, innerToken));
        }


        /// <summary>
        /// Request Method:POST
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="param">对象</param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Post<TParam, TResult>(string host, string url, TParam param, Dictionary<string, string> headerValues = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, (innerClient, innerUrl, innerContent) => innerClient.PostAsync(innerUrl, innerContent));
        }

        /// <summary>
        /// Request Method:POST
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="jsonParam"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Post<TResult>(string host, string url, string jsonParam, Dictionary<string, string> headerValues = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(jsonParam??"", Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, (innerClient, innerUrl, innerContent) => innerClient.PostAsync(innerUrl, innerContent));
        }

        /// <summary>
        /// Request Method:POST
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="jsonParam"></param>
        /// <param name="token"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Post<TResult>(string host, string url, string jsonParam, CancellationToken token, Dictionary<string, string> headerValues = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(jsonParam, Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, token, (innerClient, innerUrl, innerContent, innerToken) => innerClient.PostAsync(innerUrl, innerContent, innerToken));
        }



        /// <summary>
        /// Request Method:Put
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="param">对象</param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Put<TParam, TResult>(string host, string url, TParam param, Dictionary<string, string> headerValues = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, (innerClient, innerUrl, innerContent) => innerClient.PutAsync(innerUrl, innerContent));
        }

        /// <summary>
        /// Request Method:Put
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="param">对象</param>
        /// <param name="token"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Put<TParam, TResult>(string host, string url, TParam param, CancellationToken token, Dictionary<string, string> headerValues = null)
        {
            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, token, (innerClient, innerUrl, innerContent, innerToken) => innerClient.PutAsync(innerUrl, innerContent, innerToken));
        }

        /// <summary>
        /// Request Method:Put
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="jsonParam"></param>
        /// <param name="token"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Put<TResult>(string host, string url, string jsonParam, CancellationToken token, Dictionary<string, string> headerValues = null)
        {

            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(jsonParam, Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, token, (innerClient, innerUrl, innerContent, innerToken) => innerClient.PutAsync(innerUrl, innerContent, innerToken));
        }
        /// <summary>
        /// Request Method:Put
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="host">域名</param>
        /// <param name="url">路由</param>
        /// <param name="jsonParam"></param>
        /// <param name="headerValues">Request Headers</param>
        /// <returns></returns>
        public static TResult Put<TResult>(string host, string url, string jsonParam, Dictionary<string, string> headerValues = null)
        {

            HttpClient client = GetHttpClient(host);
            AppendHeader(client, headerValues);
            SetHttps(host);
            HttpContent contentPost = new StringContent(jsonParam, Encoding.UTF8, "application/json");
            return Request<TResult>(client, url, contentPost, (innerClient, innerUrl, innerContent) => innerClient.PutAsync(innerUrl, innerContent));
        }


        private static HttpClient GetHttpClient(string host)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(host),
                Timeout = _timeout
            };
        }

        private static void AppendHeader(HttpClient httpClient, Dictionary<string, string> headerValues)
        {
            if (headerValues == null || headerValues.Count == 0)
                return;
            foreach (var headerValue in headerValues)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(headerValue.Key, new List<string> { headerValue.Value });
            }
        }

        private static string AppendParamToUrl(string url, Dictionary<string, string> paramters)
        {
            // URL参数
            if (paramters != null && paramters.Count != 0)
            {
                url = url + "?";
                url = paramters.Aggregate(url, (current, item) => current + item.Key + "=" + item.Value + "&");
                url = url.Remove(url.LastIndexOf("&", StringComparison.Ordinal), 1);
            }
            return url;
        }


        private static TResult Request<TResult>(HttpClient client, string url, Func<HttpClient, string, Task<HttpResponseMessage>> getMessage)
        {
            var resp = getMessage(client, url).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsAsync<TResult>().Result;
            return result;
        }

        private static TResult Request<TResult>(HttpClient client, string url, CancellationToken token, Func<HttpClient, string, CancellationToken, Task<HttpResponseMessage>> getMessage)
        {
            var resp = getMessage(client, url, token).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsAsync<TResult>(token).Result;
            return result;
        }

        private static TResult Request<TResult>(HttpClient client, string url, HttpContent content, Func<HttpClient, string, HttpContent, Task<HttpResponseMessage>> getMessage)
        {
            var resp = getMessage(client, url, content).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsAsync<TResult>().Result;
            return result;
        }

        private static TResult Request<TResult>(HttpClient client, string url, HttpContent content, CancellationToken token, Func<HttpClient, string, HttpContent, CancellationToken, Task<HttpResponseMessage>> getMessage)
        {
            var resp = getMessage(client, url, content, token).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsAsync<TResult>(token).Result;
            return result;
        }


        private static void SetHttps(string host)
        {
            if (host.StartsWith("Https", StringComparison.CurrentCultureIgnoreCase))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }
        }


        #endregion
        /// <summary>
        /// 获取http文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Stream GetFile(string url)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(url),
                Timeout = _timeout
            };

            SetHttps(url);

            var resp = httpClient.GetAsync(url).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsStreamAsync().Result;
            //result.Seek(0, SeekOrigin.Begin);
            return result;
        }

    }
}
