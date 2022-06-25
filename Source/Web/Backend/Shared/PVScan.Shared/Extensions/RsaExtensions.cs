using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Extensions
{
    public static class RsaExtensions
    {
        public static RSACryptoServiceProvider ParsePrivateKey(string filePath)
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            var allLines = File.ReadAllLines(filePath);
            var keyBytes = ExtractKeyBytes(allLines);

            using var rsa = RSA.Create(1024);
            rsa.ImportRSAPrivateKey(keyBytes, out _);
            
            return ExportRSA(rsa, true);
        }

        public static RSACryptoServiceProvider ParsePublicKey(string filePath)
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            var allLines = File.ReadAllLines(filePath);
            var keyBytes = ExtractKeyBytes(allLines);

            using var rsa = RSA.Create(1024);
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);

            return ExportRSA(rsa, false);
        }

        private static byte[] ExtractKeyBytes(IEnumerable<string> allLines)
        {
            var res = allLines.Skip(1).SkipLast(1);
            var oneStringRes = res.Aggregate("", (s1, s2) => s1 + s2).Trim();
            var keyBytes = Convert.FromBase64String(oneStringRes);
            return keyBytes;
        }

        private static RSACryptoServiceProvider ExportRSA(RSA rsa, bool exportParameters)
        {
            var rsaParams = rsa.ExportParameters(exportParameters);

            var cryptoServiceProvider = new RSACryptoServiceProvider();
            cryptoServiceProvider.ImportParameters(rsaParams);
            return cryptoServiceProvider;
        }
    }
}
