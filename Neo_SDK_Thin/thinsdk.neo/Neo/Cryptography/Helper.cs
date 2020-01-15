using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThinSdk.Neo.Cryptography
{
    public static class Helper
    {
        [ThreadStatic]
        static System.Security.Cryptography.SHA256 _sha256;
        public static System.Security.Cryptography.SHA256 Sha256
        {
            get
            {
                if (_sha256 == null)
                    _sha256 = System.Security.Cryptography.SHA256.Create();
                return _sha256;
            }
        }
        [ThreadStatic]
        static Neo.Cryptography.RIPEMD160Managed _ripemd160;
        public static Neo.Cryptography.RIPEMD160Managed RIPEMD160
        {
            get
            {
                if (_ripemd160 == null)
                    _ripemd160 = new Neo.Cryptography.RIPEMD160Managed();
                return _ripemd160;
            }
        }

        public static byte[] CalcSha256(byte[] data, int start = 0, int length = -1)
        {
            byte[] tdata = null;

            if (start == 0 && length == -1)
            {
                tdata = data;
            }
            else
            {
                tdata = new byte[length];
                Array.Copy(data, 0, tdata, 0, length);
            }
            return Sha256.ComputeHash(tdata);

        }
        public static byte[] Base58CheckDecode(string input)
        {
            byte[] buffer = ThinSdk.Neo.Cryptography.Base58.Decode(input);
            if (buffer.Length < 4) throw new FormatException();

            var b1 = CalcSha256(buffer, 0, buffer.Length - 4);

            byte[] checksum = CalcSha256(b1);

            if (!buffer.Skip(buffer.Length - 4).SequenceEqual(checksum.Take(4)))
                throw new FormatException();
            return buffer.Take(buffer.Length - 4).ToArray();
        }
        public static string Base58CheckEncode(byte[] data)
        {
            var b1 = CalcSha256(data);
            byte[] checksum = CalcSha256(b1);
            byte[] buffer = new byte[data.Length + 4];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            Buffer.BlockCopy(checksum, 0, buffer, data.Length, 4);
            return ThinSdk.Neo.Cryptography.Base58.Encode(buffer);
        }
        internal static byte[] AES256Encrypt(byte[] block, byte[] key)
        {
            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Key = key;
                aes.Mode = System.Security.Cryptography.CipherMode.ECB;
                aes.Padding = System.Security.Cryptography.PaddingMode.None;
                using (System.Security.Cryptography.ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(block, 0, block.Length);
                }
            }
        }
        internal static byte[] AES256Decrypt(byte[] block, byte[] key)
        {
            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Key = key;
                aes.Mode = System.Security.Cryptography.CipherMode.ECB;
                aes.Padding = System.Security.Cryptography.PaddingMode.None;
                using (System.Security.Cryptography.ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(block, 0, block.Length);
                }
            }
        }
        public static byte[] XOR(byte[] x, byte[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException();
            return x.Zip(y, (a, b) => (byte)(a ^ b)).ToArray();
        }
    }
}
