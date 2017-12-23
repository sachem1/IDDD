using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Coralcode.Framework.Serializers
{
    /// <summary>
    /// JSON序列化和反序列化辅助类
    /// </summary>
    public class JsonObjectSerializer
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {
            try
            {
                var jsSettings = new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var jsonString =JsonConvert.SerializeObject(t, jsSettings);
                //var ser = new DataContractJsonSerializer(typeof(T));
                //var ms = new MemoryStream();
                //ser.WriteObject(ms, t);
                //string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                //ms.Close();
                ////替换Json的Date字符串
                ////const string p = @"\\/Date\((\d+)\+\d+\)\\/";
                //var matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
                //var reg = new Regex(p);
                //jsonString = reg.Replace(jsonString, matchEvaluator);
                return jsonString;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string JsonSerializer(object data, Type type)
        {
            try
            {
                var jsSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    
                };
                var jsonString = JsonConvert.SerializeObject(data,jsSettings);
                //var ser = new DataContractJsonSerializer(typeof(T));
                //var ms = new MemoryStream();
                //ser.WriteObject(ms, t);
                //string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                //ms.Close();
                ////替换Json的Date字符串
                ////const string p = @"\\/Date\((\d+)\+\d+\)\\/";
                //var matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
                //var reg = new Regex(p);
                //jsonString = reg.Replace(jsonString, matchEvaluator);
                return jsonString;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString) where T : class
        {
            if (string.IsNullOrEmpty(jsonString))
                return null;
            try
            {
                //string p1 = @"[\d]{10}";
                //MatchEvaluator matchEvaluator1 = new MatchEvaluator(ConvertTimeStampToJsonData);
                //Regex reg1 = new Regex(p1);
                //jsonString = reg1.Replace(jsonString, matchEvaluator1);

                //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
                //const string p = @"\d{2}/\d{2}/\d{4}\s\d{2}:\d{2}:\d{2}";
                //var matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                //var reg = new Regex(p);
                //jsonString = reg.Replace(jsonString, matchEvaluator);
                //var ser = new DataContractJsonSerializer(typeof(T));
                //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                //var obj = (T)ser.ReadObject(ms);
                //return obj;
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static object JsonDeserialize(string jsonString, Type type)
        {
            if (string.IsNullOrEmpty(jsonString))
                return null;
            try
            {
                //string p1 = @"[\d]{10}";
                //MatchEvaluator matchEvaluator1 = new MatchEvaluator(ConvertTimeStampToJsonData);
                //Regex reg1 = new Regex(p1);
                //jsonString = reg1.Replace(jsonString, matchEvaluator1);

                //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
                //const string p = @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}";
                //var matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                //var reg = new Regex(p);
                //jsonString = reg.Replace(jsonString, matchEvaluator);
                //var ser = new DataContractJsonSerializer(type);
                //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                //var obj = ser.ReadObject(ms);
                //return obj;
                return JsonConvert.DeserializeObject(jsonString, type);
              
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            try
            {
                var dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            var dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            var ts = dt - DateTime.Parse("1970-01-01");
            return string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
        }
    }
}