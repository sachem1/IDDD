using System;
using System.Text;

namespace Coralcode.Framework.Security
{
    public partial class CoralSecurity
    {
        //public static readonly byte[] AES_IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        #region AES

        /// <summary>
        /// ase加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">大于16位自动截断</param>
        /// <returns></returns>
        public static string AesEncrypt(string input, string key)
        {
            if (key.Length > 16)
            {
                key = key.Substring(0, 16);
            }

            Byte[] keyArray = Encoding.UTF8.GetBytes(key);
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(input);
            System.Security.Cryptography.RijndaelManaged rDel = new System.Security.Cryptography.RijndaelManaged
            {
                Key = keyArray,
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };
            System.Security.Cryptography.ICryptoTransform cTransform = rDel.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);


        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">大于16位自动截断</param>
        /// <returns></returns>
        public static string AesDecrypt(string input, string key)
        {
            if (key.Length > 16)
            {
                key = key.Substring(0, 16);
            }

            Byte[] keyArray = Encoding.UTF8.GetBytes(key);
            Byte[] toEncryptArray = Convert.FromBase64String(input);
            System.Security.Cryptography.RijndaelManaged rDel = new System.Security.Cryptography.RijndaelManaged
            {
                Key = keyArray,
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };
            System.Security.Cryptography.ICryptoTransform cTransform = rDel.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);

            
        }
        #endregion
    }
}