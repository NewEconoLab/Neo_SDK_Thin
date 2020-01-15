using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace ThinSdk.Neo.VM
{
    public class ScriptBuilder : IDisposable
    {
        private readonly MemoryStream ms = new MemoryStream();
        private readonly BinaryWriter writer;

        public int Offset => (int)ms.Position;

        public ScriptBuilder()
        {
            writer = new BinaryWriter(ms);
        }

        public void Dispose()
        {
            writer.Dispose();
            ms.Dispose();
        }

        public ScriptBuilder Emit(OpCode op, byte[] arg = null)
        {
            writer.Write((byte)op);
            if (arg != null)
                writer.Write(arg);
            return this;
        }

        public ScriptBuilder EmitJump(OpCode op, short offset)
        {
            if (op != OpCode.JMP && op != OpCode.JMPIF && op != OpCode.JMPIFNOT && op != OpCode.CALL)
                throw new ArgumentException();
            return Emit(op, BitConverter.GetBytes(offset));
        }

        public ScriptBuilder EmitPush(object obj)
        {
            switch (obj)
            {
                case UInt160 data:
                    return EmitPush(data.data);
                case UInt256 data:
                    return EmitPush(data.data);
                case bool data:
                    return EmitPush(data);
                case byte[] data:
                    return EmitPush(data);
                case string data:
                    return EmitPush(data);
                case BigInteger data:
                    return EmitPush(data);
                case sbyte data:
                    return EmitPush(data);
                case byte data:
                    return EmitPush(data);
                case short data:
                    return EmitPush(data);
                case ushort data:
                    return EmitPush(data);
                case int data:
                    return EmitPush(data);
                case uint data:
                    return EmitPush(data);
                case long data:
                    return EmitPush(data);
                case ulong data:
                    return EmitPush(data);
                case Enum data:
                    return EmitPush(BigInteger.Parse(data.ToString("d")));
                default:
                    throw new ArgumentException();
            }
        }

        public ScriptBuilder EmitPush(BigInteger number)
        {
            if (number == -1) return Emit(OpCode.PUSHM1);
            if (number == 0) return Emit(OpCode.PUSH0);
            if (number > 0 && number <= 16) return Emit(OpCode.PUSH1 - 1 + (byte)number);
            return EmitPush(number.ToByteArray());
        }

        public ScriptBuilder EmitPush(bool data)
        {
            return Emit(data ? OpCode.PUSH1 : OpCode.PUSH0);
        }

        public ScriptBuilder EmitPush(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            if (data.Length < 0x100)
            {
                Emit(OpCode.PUSHDATA1);
                writer.Write((byte)data.Length);
                writer.Write(data);
            }
            else if (data.Length < 0x10000)
            {
                Emit(OpCode.PUSHDATA2);
                writer.Write((ushort)data.Length);
                writer.Write(data);
            }
            else// if (data.Length < 0x100000000L)
            {
                Emit(OpCode.PUSHDATA4);
                writer.Write(data.Length);
                writer.Write(data);
            }
            return this;
        }

        public ScriptBuilder EmitPush(string data)
        {
            return EmitPush(Encoding.UTF8.GetBytes(data));
        }

        public ScriptBuilder EmitSysCall(uint api)
        {
            return Emit(OpCode.SYSCALL, BitConverter.GetBytes(api));
        }

        public ScriptBuilder EmitSysCall(string method)
        {
            if (method == null)
                throw new ArgumentNullException();
            uint api = BitConverter.ToUInt32(Neo.Cryptography.Helper.Sha256.ComputeHash(Encoding.ASCII.GetBytes(method)), 0);
            return EmitSysCall(api);
        }

        public ScriptBuilder EmitSysCall(string method, params object[] args)
        {
            for (int i = args.Length - 1; i >= 0; i--)
                EmitPush(args[i]);
            return EmitSysCall(method);
        }

        public byte[] ToArray()
        {
            writer.Flush();
            return ms.ToArray();
        }
    }
}
