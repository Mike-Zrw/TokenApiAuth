using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ApiTokenAuth.Helper
{
    /// <summary>
    /// 非对称加密
    /// </summary>
    public class RSAHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_inputString">需要加密的字符串信息</param>
        /// <param name="p_strKeyStr">加密用的密钥字符串(*.cyh_publickey)</param>
        /// <returns>加密以后的字符串信息</returns>
        public static string Encrypt(string p_inputString, string p_strKeyStr)
        {
            string outString = null;
            string bitStrengthString = p_strKeyStr.Substring(0, p_strKeyStr.IndexOf("</BitStrength>") + 14);
            p_strKeyStr = p_strKeyStr.Replace(bitStrengthString, "");
            int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
            outString = EncryptString(p_inputString, bitStrength, p_strKeyStr);
            return outString;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_inputString">需要解密的字符串信息</param>
        /// <param name="p_strKeyPath">解密用的密钥字符串(*.cyh_primarykey)</param>
        /// <returns>解密以后的字符串信息</returns>
        public static string Decrypt(string p_inputString, string p_strKeyStr)
        {
            string outString = null;
            string bitStrengthString = p_strKeyStr.Substring(0, p_strKeyStr.IndexOf("</BitStrength>") + 14);
            p_strKeyStr = p_strKeyStr.Replace(bitStrengthString, "");
            int bitStrength = Convert.ToInt32(bitStrengthString.Replace("<BitStrength>", "").Replace("</BitStrength>", ""));
            outString = DecryptString(p_inputString, bitStrength, p_strKeyStr);
            return outString;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_inputString">需要加密的字符串</param>
        /// <param name="p_dwKeySize">密钥的大小</param>
        /// <param name="p_xmlString">包含密钥的XML文本信息</param>
        /// <returns>加密后的文本信息</returns>
        private static string EncryptString(string p_inputString, int p_dwKeySize, string p_xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(p_dwKeySize);
            FromXmlString(rsaCryptoServiceProvider, p_xmlString);
            int keySize = p_dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(p_inputString);
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
                Array.Reverse(encryptedBytes);
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_inputString">需要解密的字符串信息</param>
        /// <param name="p_dwKeySize">密钥的大小</param>
        /// <param name="p_xmlString">包含密钥的文本信息</param>
        /// <returns>解密后的文本信息</returns>
        private static string DecryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            FromXmlString(rsaCryptoServiceProvider,xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }
        public static void FromXmlString(RSA rsa, string xml)
        {
            var csp = ExtractFromXml(xml);
            rsa.ImportParameters(csp);
        }

        public static RSAParameters ExtractFromXml(string xml)
        {
            var csp = new RSAParameters();
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element)
                        continue;

                    var elName = reader.Name;
                    if (elName == "RSAKeyValue")
                        continue;

                    do
                    {
                        reader.Read();
                    } while (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.EndElement);

                    if (reader.NodeType == XmlNodeType.EndElement)
                        continue;

                    var value = reader.Value;
                    switch (elName)
                    {
                        case "Modulus":
                            csp.Modulus = Convert.FromBase64String(value);
                            break;
                        case "Exponent":
                            csp.Exponent = Convert.FromBase64String(value);
                            break;
                        case "P":
                            csp.P = Convert.FromBase64String(value);
                            break;
                        case "Q":
                            csp.Q = Convert.FromBase64String(value);
                            break;
                        case "DP":
                            csp.DP = Convert.FromBase64String(value);
                            break;
                        case "DQ":
                            csp.DQ = Convert.FromBase64String(value);
                            break;
                        case "InverseQ":
                            csp.InverseQ = Convert.FromBase64String(value);
                            break;
                        case "D":
                            csp.D = Convert.FromBase64String(value);
                            break;
                    }
                }

                return csp;
            }
        }

    }
}
