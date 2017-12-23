using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Coralcode.Framework.Security
{
    public partial class CoralSecurity
    {
        /// <summary>
        /// 创建公钥，秘钥对
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> Sha1CreateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string privateKey = rsa.ToXmlString(true);
            string publicKey = rsa.ToXmlString(false);

            return new KeyValuePair<string, string>(publicKey, privateKey);
        }

        /// <summary>
        /// 数字签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>签名</returns>
        public static string Sha1HashAndSignString(string plaintext, string privateKey)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plaintext);

            using (RSACryptoServiceProvider rsAalg = new RSACryptoServiceProvider())
            {
                rsAalg.FromXmlString(privateKey);
                //使用SHA1进行摘要算法，生成签名
                byte[] encryptedData = rsAalg.SignData(dataToEncrypt, new SHA1CryptoServiceProvider());
                return Convert.ToBase64String(encryptedData);
            }
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="signedData">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static bool Sha1VerifySigned(string plaintext, string signedData, string publicKey)
        {
            using (RSACryptoServiceProvider rsAalg = new RSACryptoServiceProvider())
            {
                rsAalg.FromXmlString(publicKey);
                
                byte[] dataToVerifyBytes = Encoding.UTF8.GetBytes(plaintext);

                
                byte[] signedDataBytes = Convert.FromBase64String(signedData);
                return rsAalg.VerifyData(dataToVerifyBytes, new SHA1CryptoServiceProvider(), signedDataBytes);
            }
        } 


        public static string XmlPrivateKeyToPemg(string privateKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            var p = rsa.ExportParameters(true);
            var key = new RsaPrivateCrtKeyParameters(
                new BigInteger(1, p.Modulus), new BigInteger(1, p.Exponent), new BigInteger(1, p.D),
                new BigInteger(1, p.P), new BigInteger(1, p.Q), new BigInteger(1, p.DP), new BigInteger(1, p.DQ),
                new BigInteger(1, p.InverseQ));

            using (var sw = new StringWriter())
            {
                var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                pemWriter.WriteObject(key);
                return sw.ToString();
            }

        }

        public static string PemPrivateKeyToXml(string privateKey)
        {
            AsymmetricCipherKeyPair keyPair;
            using (var sr = new StringReader(privateKey))
            {
                var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            }
            var key = (RsaPrivateCrtKeyParameters)keyPair.Private;
            var p = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.PublicExponent.ToByteArrayUnsigned(),
                D = key.Exponent.ToByteArrayUnsigned(),
                P = key.P.ToByteArrayUnsigned(),
                Q = key.Q.ToByteArrayUnsigned(),
                DP = key.DP.ToByteArrayUnsigned(),
                DQ = key.DQ.ToByteArrayUnsigned(),
                InverseQ = key.QInv.ToByteArrayUnsigned(),
            };
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(p);
            return rsa.ToXmlString(true);
        }


        public static string XmlPublicKeyToPemg(string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            var p = rsa.ExportParameters(false);
            var key = new RsaKeyParameters(false,new BigInteger(1, p.Modulus), new BigInteger(1, p.Exponent));

            using (var sw = new StringWriter())
            {
                var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                pemWriter.WriteObject(key);
                return sw.ToString();
            }

        }

        public static string PemPublicKeyToXml(string publicKey)
        {
            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters key;
            using (var sr = new StringReader(publicKey))
            {
                var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                key = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)pemReader.ReadObject();
            }
          
            var p = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.Exponent.ToByteArrayUnsigned(),
            };
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(p);
            return rsa.ToXmlString(false);
        }
    }
}