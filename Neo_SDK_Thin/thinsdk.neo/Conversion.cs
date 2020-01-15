using System;
using System.Linq;
using System.Text;
using ThinSdk.Neo;
using ThinSdk.Neo.Cryptography;
using ThinSdk.Neo.VM;
using Hepler = ThinSdk.Neo.Cryptography.Helper;

namespace ThinSdk
{
    public static class Conversion
    {
        public static UInt256 CalcHash256(byte[] data)
        {
            var hash1 = Hepler.Sha256.ComputeHash(data);
            var hash2 = Hepler.Sha256.ComputeHash(hash1);
            return hash2;
        }

        public static UInt160 CalcHash160(byte[] data)
        {
            var hash1 = Hepler.Sha256.ComputeHash(data);
            var hash2 = Hepler.RIPEMD160.ComputeHash(hash1);
            return hash2;
        }

        public static string Bytes2HexString(this byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
        }

        public static byte[] HexString2Bytes(this string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }

        public static string PrivateKey2Wif(this byte[] prikey)
        {
            if (prikey.Length != 32)
                throw new Exception("error prikey.");
            byte[] data = new byte[34];
            data[0] = 0x80;
            data[33] = 0x01;
            for (var i = 0; i < 32; i++)
            {
                data[i + 1] = prikey[i];
            }
            byte[] checksum = Hepler.Sha256.ComputeHash(data);
            checksum = Hepler.Sha256.ComputeHash(checksum);
            checksum = checksum.Take(4).ToArray();
            byte[] alldata = data.Concat(checksum).ToArray();
            string wif = Base58.Encode(alldata);
            return wif;
        }

        public static byte[] WIF2PrivateKey(string wif)
        {
            if (wif == null) throw new ArgumentNullException();
            byte[] data = Base58.Decode(wif);
            //检查标志位
            if (data.Length != 38 || data[0] != 0x80 || data[33] != 0x01)
                throw new Exception("wif length or tag is error");
            //取出检验字节
            var sum = data.Skip(data.Length - 4);
            byte[] realdata = data.Take(data.Length - 4).ToArray();

            //验证,对前34字节进行进行两次hash取前4个字节
            byte[] checksum = Hepler.Sha256.ComputeHash(realdata);
            checksum = Hepler.Sha256.ComputeHash(checksum);
            var sumcalc = checksum.Take(4);
            if (sum.SequenceEqual(sumcalc) == false)
                throw new Exception("the sum is not match.");

            byte[] privateKey = new byte[32];
            Buffer.BlockCopy(data, 1, privateKey, 0, privateKey.Length);
            Array.Clear(data, 0, data.Length);
            return privateKey;
        }

        public static byte[] PrivateKey2PublicKey(this byte[] privateKey)
        {
            var PublicKey = ThinSdk.Neo.Cryptography.ECC.ECCurve.Secp256r1.G * privateKey;
            return PublicKey.EncodePoint(true);
        }

        public static byte[] PrivateKey2PublicKey_EncodePointFalse(this byte[] privateKey)
        {
            var PublicKey = ThinSdk.Neo.Cryptography.ECC.ECCurve.Secp256r1.G * privateKey;
            return PublicKey.EncodePoint(false);
        }

        public static byte[] PublicKey2AddressScript(this byte[] publicKey)
        {
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(publicKey);
                sb.Emit(OpCode.PUSHNULL);
                sb.EmitSysCall("Neo.Crypto.ECDsaVerify");
                return sb.ToArray();
            }
        }

        public static UInt160 PublicKey2ScriptHash(this byte[] publicKey)
        {
            byte[] script = publicKey.PublicKey2AddressScript();
            return CalcHash160(script);
        }

        public static string ScriptHash2Address(this UInt160 scripthash)
        {
            byte[] data = new byte[20 + 1];
            data[0] = 0x35;
            Array.Copy(scripthash, 0, data, 1, 20);
            var hash = Hepler.Sha256.ComputeHash(data);
            hash = Hepler.Sha256.ComputeHash(hash);

            var alldata = data.Concat(hash.Take(4)).ToArray();

            return Base58.Encode(alldata);
        }

        public static string PublicKey2Address(this byte[] publicKey)
        {
            var script = publicKey.PublicKey2AddressScript();
            var hash = CalcHash160(script);
            var address = hash.ScriptHash2Address();
            return address;
        }

        public static UInt160 Address2ScriptHash(this string address)
        {
            var alldata = Base58.Decode(address);
            if (alldata.Length != 25)
                throw new Exception("error length.");
            var data = alldata.Take(alldata.Length - 4).ToArray();
            if (data[0] != 0x35)
                throw new Exception("not a address");
            var hash = Hepler.Sha256.ComputeHash(data);
            hash = Hepler.Sha256.ComputeHash(hash);
            var hashbts = hash.Take(4).ToArray();
            var datahashbts = alldata.Skip(alldata.Length - 4).ToArray();
            if (hashbts.SequenceEqual(datahashbts) == false)
                throw new Exception("not match hash");
            var pkhash = data.Skip(1).ToArray();
            return new UInt160(pkhash);
        }

        public static UInt160 Address2ScriptHash_WithoutCheck(this string address)
        {
            var alldata = Base58.Decode(address);
            if (alldata.Length != 25)
                throw new Exception("error length.");
            if (alldata[0] != 0x35)
                throw new Exception("not a address");
            var data = alldata.Take(alldata.Length - 4).ToArray();
            var pkhash = data.Skip(1).ToArray();
            return new UInt160(pkhash);
        }

        public static string PrivateKey2Nep2(this byte[] prikey, string passphrase)
        {
            var pubkey = prikey.PrivateKey2PublicKey();
            var script_hash = pubkey.PublicKey2ScriptHash();

            string address = script_hash.ScriptHash2Address();

            var b1 = Hepler.CalcSha256(Encoding.ASCII.GetBytes(address));
            var b2 = Hepler.CalcSha256(b1);
            byte[] addresshash = b2.Take(4).ToArray();
            byte[] derivedkey = Neo.Cryptography.SCrypt.DeriveKey(Encoding.UTF8.GetBytes(passphrase), addresshash, 16384, 8, 8, 64);
            byte[] derivedhalf1 = derivedkey.Take(32).ToArray();
            byte[] derivedhalf2 = derivedkey.Skip(32).ToArray();
            var xorinfo = Hepler.XOR(prikey, derivedhalf1);
            byte[] encryptedkey = Hepler.AES256Encrypt(xorinfo, derivedhalf2);
            byte[] buffer = new byte[39];
            buffer[0] = 0x01;
            buffer[1] = 0x42;
            buffer[2] = 0xe0;
            Buffer.BlockCopy(addresshash, 0, buffer, 3, addresshash.Length);
            Buffer.BlockCopy(encryptedkey, 0, buffer, 7, encryptedkey.Length);
            return Hepler.Base58CheckEncode(buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static UInt160 Script2ScriptHash(this byte[] script)
        {
            var scripthash = Hepler.Sha256.ComputeHash(script);
            scripthash = Hepler.RIPEMD160.ComputeHash(scripthash);
            return scripthash;
        }

        public static byte[] NEP22PrivateKey(this string nep2, string passphrase, int N = 16384, int r = 8, int p = 8)
        {
            if (nep2 == null) throw new ArgumentNullException(nameof(nep2));
            if (passphrase == null) throw new ArgumentNullException(nameof(passphrase));
            byte[] data = Hepler.Base58CheckDecode(nep2);
            if (data.Length != 39 || data[0] != 0x01 || data[1] != 0x42 || data[2] != 0xe0)
                throw new FormatException();
            byte[] addresshash = new byte[4];
            Buffer.BlockCopy(data, 3, addresshash, 0, 4);
            byte[] derivedkey = Neo.Cryptography.SCrypt.DeriveKey(Encoding.UTF8.GetBytes(passphrase), addresshash, N, r, p, 64);
            byte[] derivedhalf1 = derivedkey.Take(32).ToArray();
            byte[] derivedhalf2 = derivedkey.Skip(32).ToArray();
            byte[] encryptedkey = new byte[32];
            Buffer.BlockCopy(data, 7, encryptedkey, 0, 32);
            byte[] prikey = Hepler.XOR(Hepler.AES256Decrypt(encryptedkey, derivedhalf2), derivedhalf1);
            var pubkey = prikey.PrivateKey2PublicKey();
            var address = pubkey.PublicKey2Address();
            var hash = Hepler.CalcSha256(Encoding.ASCII.GetBytes(address));
            hash = Hepler.CalcSha256(hash);
            for (var i = 0; i < 4; i++)
            {
                if (hash[i] != addresshash[i])
                    throw new Exception("check error.");
            }
            return prikey;
        }
    }
}
