using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSATest
{
    public class RSAHelper
    {

        /// <summary>
        /// 创建，标准的RSA密钥
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GeneratRsaKey()
        {
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            RsaKeyGenerationParameters rsaKeyGenerationParameters = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new Org.BouncyCastle.Security.SecureRandom(), 1024, 25);
            rsaKeyPairGenerator.Init(rsaKeyGenerationParameters);//初始化参数
            AsymmetricCipherKeyPair keyPair = rsaKeyPairGenerator.GenerateKeyPair();
            AsymmetricKeyParameter publicKey = keyPair.Public;//公钥
            AsymmetricKeyParameter privateKey = keyPair.Private;//私钥

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);

            Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
            byte[] publicInfoByte = asn1ObjectPublic.GetEncoded();
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded();

            var rsakeyPair = new Dictionary<string, string>();
            rsakeyPair.Add("PUBLIC", Convert.ToBase64String(publicInfoByte));
            rsakeyPair.Add("PRIVATE", Convert.ToBase64String(privateInfoByte));


            return rsakeyPair;
        }

        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="content">要加密的内容</param>
        /// <returns></returns>
        public static string encryptByPublicKey(string publicKey, string data)
        {
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(Convert.FromBase64String(publicKey));//这里也可以从流中读取，从本地导入
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(SubjectPublicKeyInfo.GetInstance(pubKeyObj));
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher.Init(true, pubKey);//true 表示加密
            var dataByte = Encoding.UTF8.GetBytes(data);

            byte[] encryptData = cipher.ProcessBlock(dataByte, 0, dataByte.Length);
            return Convert.ToBase64String(encryptData);
        }

        public static string decryptByPublicKey(string publicKey, string data)
        {
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(Convert.FromBase64String(publicKey));//这里也可以从流中读取，从本地导入
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(SubjectPublicKeyInfo.GetInstance(pubKeyObj));
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher.Init(false, pubKey);//false 表示解密

            var dataByte = Convert.FromBase64String(data);

            byte[] decryptData = cipher.ProcessBlock(dataByte, 0, dataByte.Length);
            return Encoding.UTF8.GetString(decryptData);
        }

        /// <summary>
        /// 私钥加密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string encryptByPrivateKey(string privateKey, string data)
        {
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher.Init(true, priKey);//true 表示加密
            var dataByte = Encoding.UTF8.GetBytes(data);

            return Convert.ToBase64String(cipher.ProcessBlock(dataByte, 0, dataByte.Length));

        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string decryptByPrivateKey(string privateKey, string data)
        {
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher.Init(false, priKey);//false 表示解密
            var encryptData = Convert.FromBase64String(data);
            string decryptData = Encoding.UTF8.GetString(cipher.ProcessBlock(encryptData, 0, encryptData.Length));
            return decryptData;

        }



    }
}
