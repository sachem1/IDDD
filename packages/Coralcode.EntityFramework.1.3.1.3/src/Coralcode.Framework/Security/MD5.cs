using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Coralcode.Framework.Security
{
    public partial class CoralSecurity
    {
        #region MD5函数
        /// <summary>
        /// MD5函数,需引用：using System.Security.Cryptography;
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string Md5(string str)
        {
            //微软md5方法参考return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        /// <summary>
        /// MD5函数,需引用：using System.Security.Cryptography;
        /// </summary>
        /// <param name="stream">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string Md5(Stream stream)
        {
            // 把 Stream 转换成 byte[]
            byte[] b = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(b, 0, b.Length);
            //微软md5方法参考return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }


        /// <summary>
        /// MD5函数,需引用：using System.Security.Cryptography;
        /// </summary>
        /// <param name="bytes">原始字节</param>
        /// <returns>MD5结果</returns>
        public static string Md5(byte[] bytes)
        {
            //微软md5方法参考return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        #endregion
    }
}
