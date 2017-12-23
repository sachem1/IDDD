using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Coralcode.Framework.Security
{
    public partial class CoralSecurity
    {
        #region MD5����
        /// <summary>
        /// MD5����,�����ã�using System.Security.Cryptography;
        /// </summary>
        /// <param name="str">ԭʼ�ַ���</param>
        /// <returns>MD5���</returns>
        public static string Md5(string str)
        {
            //΢��md5�����ο�return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        /// <summary>
        /// MD5����,�����ã�using System.Security.Cryptography;
        /// </summary>
        /// <param name="stream">ԭʼ�ַ���</param>
        /// <returns>MD5���</returns>
        public static string Md5(Stream stream)
        {
            // �� Stream ת���� byte[]
            byte[] b = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(b, 0, b.Length);
            //΢��md5�����ο�return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }


        /// <summary>
        /// MD5����,�����ã�using System.Security.Cryptography;
        /// </summary>
        /// <param name="bytes">ԭʼ�ֽ�</param>
        /// <returns>MD5���</returns>
        public static string Md5(byte[] bytes)
        {
            //΢��md5�����ο�return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        #endregion
    }
}
