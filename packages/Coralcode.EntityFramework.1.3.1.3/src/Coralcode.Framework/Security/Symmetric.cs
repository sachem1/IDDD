using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Coralcode.Framework.Security
{
    public partial class CoralSecurity
    {
        private readonly SymmetricAlgorithm _mobjCryptoService = new RijndaelManaged();
        private string Key= "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
       

        /// <summary>   
        /// 获得密钥   
        /// </summary>   
        /// <returns>密钥</returns>   
        private byte[] SymmetricGetLegalKey()
        {
            string sTemp = Key;
            _mobjCryptoService.GenerateKey();
            byte[] bytTemp = _mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return Encoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>   
        /// 获得初始向量IV   
        /// </summary>   
        /// <returns>初试向量IV</returns>   
        private byte[] SymmetricGetLegalIv()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            _mobjCryptoService.GenerateIV();
            byte[] bytTemp = _mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return Encoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>   
        /// 加密方法   
        /// </summary>   
        /// <param name="source">待加密的串</param>   
        /// <returns>经过加密的串</returns>   
        public string SymmetricEncrypt(string source)
        {
            byte[] bytIn = Encoding.UTF8.GetBytes(source);
            MemoryStream ms = new MemoryStream();
            _mobjCryptoService.Key = SymmetricGetLegalKey();
            _mobjCryptoService.IV = SymmetricGetLegalIv();
            ICryptoTransform encrypto = _mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }
        /// <summary>   
        /// 解密方法   
        /// </summary>   
        /// <param name="source">待解密的串</param>   
        /// <returns>经过解密的串</returns>   
        public string SymmetricDecrypt(string source)
        {
            byte[] bytIn = Convert.FromBase64String(source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            _mobjCryptoService.Key = SymmetricGetLegalKey();
            _mobjCryptoService.IV = SymmetricGetLegalIv();
            ICryptoTransform encrypto = _mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }



    }
}
