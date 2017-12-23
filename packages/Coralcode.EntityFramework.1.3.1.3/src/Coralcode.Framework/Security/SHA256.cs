using System;
using System.Security.Cryptography;
using System.Text;

namespace Coralcode.Framework.Security
{
    public partial class CoralSecurity
    {
        #region SHA256函数
        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string Sha256(string str)
        {
            byte[] sha256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed sha256 = new SHA256Managed();
            byte[] result = sha256.ComputeHash(sha256Data);
            return Convert.ToBase64String(result);  //返回长度为44字节的字符串
        }
        #endregion
    }
}
