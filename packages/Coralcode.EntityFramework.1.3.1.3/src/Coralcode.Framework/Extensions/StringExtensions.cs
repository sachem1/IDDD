using System.Security.Cryptography;
using System.Text;

namespace Coralcode.Framework.Extensions
{
    public static class StringExtensions
    {
        public static string ToMD5Hash(this string inputString)
        {
            var md5 = new MD5CryptoServiceProvider();

            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));

            var sb = new StringBuilder();

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }


        public static long? ToLong(this string value)
        {
            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }

        #region 全角转换半角以及半角转换为全角
        /// <summary>
        /// 转全角的函数(SBC case)
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        public static string ToSbc(this string input)
        {
            // 半角转全角：
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 32)
                {
                    array[i] = (char)12288;
                    continue;
                }
                if (array[i] < 127)
                {
                    array[i] = (char)(array[i] + 65248);
                }
            }
            return new string(array);
        }

        /// <summary>
        /// 转半角的函数(DBC case)   
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"任意字符串></param>
        /// <returns半角字符串></returns>
        public static string ToDbc(this string input)
        {
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 12288)
                {
                    array[i] = (char)32;
                    continue;
                }
                if (array[i] > 65280 && array[i] < 65375)
                {
                    array[i] = (char)(array[i] - 65248);
                }
            }
            return new string(array);
        }
        #endregion


        
    }
}
