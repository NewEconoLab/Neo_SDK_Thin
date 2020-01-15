using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using ThinSdk.Neo.VM;

namespace ThinSdk.Neo
{
    public static class Helper
    {
        [ThreadStatic]
        static System.Security.Cryptography.RandomNumberGenerator _random;
        public static System.Security.Cryptography.RandomNumberGenerator Random
        {
            get
            {
                if (_random == null)
                    _random = System.Security.Cryptography.RandomNumberGenerator.Create();
                return _random;
            }
        }
        [ThreadStatic]
        static System.Random _randomquick;
        public static System.Random RandomQuick
        {
            get
            {
                if (_randomquick == null)
                    _randomquick = new System.Random();
                return _randomquick;
            }
        }

        public static int BitLen(int w)
        {
            return (w < 1 << 15 ? (w < 1 << 7
                ? (w < 1 << 3 ? (w < 1 << 1
                ? (w < 1 << 0 ? (w < 0 ? 32 : 0) : 1)
                : (w < 1 << 2 ? 2 : 3)) : (w < 1 << 5
                ? (w < 1 << 4 ? 4 : 5)
                : (w < 1 << 6 ? 6 : 7)))
                : (w < 1 << 11
                ? (w < 1 << 9 ? (w < 1 << 8 ? 8 : 9) : (w < 1 << 10 ? 10 : 11))
                : (w < 1 << 13 ? (w < 1 << 12 ? 12 : 13) : (w < 1 << 14 ? 14 : 15)))) : (w < 1 << 23 ? (w < 1 << 19
                ? (w < 1 << 17 ? (w < 1 << 16 ? 16 : 17) : (w < 1 << 18 ? 18 : 19))
                : (w < 1 << 21 ? (w < 1 << 20 ? 20 : 21) : (w < 1 << 22 ? 22 : 23))) : (w < 1 << 27
                ? (w < 1 << 25 ? (w < 1 << 24 ? 24 : 25) : (w < 1 << 26 ? 26 : 27))
                : (w < 1 << 29 ? (w < 1 << 28 ? 28 : 29) : (w < 1 << 30 ? 30 : 31)))));
        }
        public static int GetBitLength(this BigInteger i)
        {
            byte[] b = i.ToByteArray();
            return (b.Length - 1) * 8 + BitLen(i.Sign > 0 ? b[b.Length - 1] : 255 - b[b.Length - 1]);
        }
        public static int GetLowestSetBit(this BigInteger i)
        {
            if (i.Sign == 0)
                return -1;
            byte[] b = i.ToByteArray();
            int w = 0;
            while (b[w] == 0)
                w++;
            for (int x = 0; x < 8; x++)
                if ((b[w] & 1 << x) > 0)
                    return x + w * 8;
            throw new Exception();
        }
        public static BigInteger Mod(this BigInteger x, BigInteger y)
        {
            x %= y;
            if (x.Sign < 0)
                x += y;
            return x;
        }

        public static bool TestBit(this BigInteger i, int index)
        {
            return (i & (BigInteger.One << index)) > BigInteger.Zero;
        }
        internal static BigInteger ModInverse(this BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        internal static BigInteger NextBigIntegerQuick(int sizeInBits)
        {
            if (sizeInBits < 0)
                throw new ArgumentException("sizeInBits must be non-negative");
            if (sizeInBits == 0)
                return 0;
            byte[] b = new byte[sizeInBits / 8 + 1];
            RandomQuick.NextBytes(b);
            if (sizeInBits % 8 == 0)
                b[b.Length - 1] = 0;
            else
                b[b.Length - 1] &= (byte)((1 << sizeInBits % 8) - 1);
            return new BigInteger(b);
        }
        public static BigInteger NextBigInteger(int sizeInBits)
        {
            if (sizeInBits < 0)
                throw new ArgumentException("sizeInBits must be non-negative");
            if (sizeInBits == 0)
                return 0;
            byte[] b = new byte[sizeInBits / 8 + 1];
            Random.GetBytes(b);
            if (sizeInBits % 8 == 0)
                b[b.Length - 1] = 0;
            else
                b[b.Length - 1] &= (byte)((1 << sizeInBits % 8) - 1);
            return new BigInteger(b);
        }

        public static byte[] RandomBytes(int size)
        {
            byte[] data = new byte[size];
            Random.GetBytes(data, 0, data.Length);
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe internal static ushort ToUInt16(this byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                return *((ushort*)pbyte);
            }
        }

        public static bool IsMultiSigContract(this byte[] script, out int m, out int n)
        {
            m = 0; n = 0;
            int i = 0;
            if (script.Length < 41) return false;
            if (script[i] > (byte)OpCode.PUSH16) return false;
            if (script[i] < (byte)OpCode.PUSH1 && script[i] != 1 && script[i] != 2) return false;
            switch (script[i])
            {
                case 1:
                    m = script[++i];
                    ++i;
                    break;
                case 2:
                    m = script.ToUInt16(++i);
                    i += 2;
                    break;
                default:
                    m = script[i++] - 80;
                    break;
            }
            if (m < 1 || m > 1024) return false;
            while (script[i] == 33)
            {
                i += 34;
                if (script.Length <= i) return false;
                ++n;
            }
            if (n < m || n > 1024) return false;
            switch (script[i])
            {
                case 1:
                    if (n != script[++i]) return false;
                    ++i;
                    break;
                case 2:
                    if (script.Length < i + 3 || n != script.ToUInt16(++i)) return false;
                    i += 2;
                    break;
                default:
                    if (n != script[i++] - 80) return false;
                    break;
            }
            if (script[i++] != (byte)OpCode.SYSCALL) return false;
            if (script.Length != i + 4) return false;
            if (BitConverter.ToUInt32(script, i) != BitConverter.ToUInt32(Neo.Cryptography.Helper.Sha256.ComputeHash(Encoding.ASCII.GetBytes("Neo.Crypto.CheckMultiSig")), 0))
                return false;
            return true;
        }

        public static bool IsSignatureContract(this byte[] script)
        {
            if (script.Length != 41) return false;
            if (script[0] != (byte)OpCode.PUSHDATA1
                || script[1] != 33
                || script[35] != (byte)OpCode.PUSHNULL
                || script[36] != (byte)OpCode.SYSCALL
                || BitConverter.ToUInt32(script, 37) != BitConverter.ToUInt32(Neo.Cryptography.Helper.Sha256.ComputeHash(Encoding.ASCII.GetBytes("Neo.Crypto.ECDsaVerify")), 0))
                return false;
            return true;
        }
    }
}
