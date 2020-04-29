using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using ThinSdk.Neo.IO;

namespace ThinSdk.Neo
{
    public class Transaction : IEquatable<Transaction>
    {
        public const int MaxTransactionSize = 102400;
        public const uint MaxValidUntilBlockIncrement = 2102400;
        /// <summary>
        /// Maximum number of attributes that can be contained within a transaction
        /// </summary>
        private const int MaxTransactionAttributes = 16;
        /// <summary>
        /// Maximum number of cosigners that can be contained within a transaction
        /// </summary>
        private const int MaxCosigners = 16;

        public byte Version;
        public uint Nonce;
        public UInt160 Sender;
        /// <summary>
        /// Distributed to NEO holders.
        /// </summary>
        public long SystemFee;
        /// <summary>
        /// Distributed to consensus nodes.
        /// </summary>
        public long NetworkFee;
        public uint ValidUntilBlock;
        public TransactionAttribute[] Attributes;
        public Cosigner[] Cosigners { get; set; }
        public byte[] Script;
        public Witness[] Witnesses { get; set; }

        /// <summary>
        /// The <c>NetworkFee</c> for the transaction divided by its <c>Size</c>.
        /// <para>Note that this property must be used with care. Getting the value of this property multiple times will return the same result. The value of this property can only be obtained after the transaction has been completely built (no longer modified).</para>
        /// </summary>
        public long FeePerByte => NetworkFee / Size;

        private UInt256 _hash = null;
        public UInt256 Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash =Conversion.CalcHash256(GetMessage());
                }
                return _hash;
            }
        }

        public const int HeaderSize =
            sizeof(byte) +  //Version
            sizeof(uint) +  //Nonce
            20 +            //Sender
            sizeof(long) +  //Gas
            sizeof(long) +  //NetworkFee
            sizeof(uint);   //ValidUntilBlock

        public int Size => HeaderSize +
            Attributes.GetVarSize() +   //Attributes
            Cosigners.GetVarSize() +    //Cosigners
            Script.GetVarSize() +       //Script
            Witnesses.GetVarSize();     //Witnesses

        public void Deserialize(BinaryReader reader)
        {
            DeserializeUnsigned(reader);
            Witnesses = reader.ReadSerializableArray<Witness>();
        }

        public void DeserializeUnsigned(BinaryReader reader)
        {
            Version = reader.ReadByte();
            if (Version > 0) throw new FormatException();
            Nonce = reader.ReadUInt32();
            Sender = reader.ReadSerializable<UInt160>();
            SystemFee = reader.ReadInt64();
            if (SystemFee < 0) throw new FormatException();
            if (SystemFee % BigInteger.Pow(10, 8) != 0) throw new FormatException();
            NetworkFee = reader.ReadInt64();
            if (NetworkFee < 0) throw new FormatException();
            if (SystemFee + NetworkFee < SystemFee) throw new FormatException();
            ValidUntilBlock = reader.ReadUInt32();
            Attributes = reader.ReadSerializableArray<TransactionAttribute>(MaxTransactionAttributes);
            Cosigners = reader.ReadSerializableArray<Cosigner>(MaxCosigners);
            if (Cosigners.Select(u => u.Account).Distinct().Count() != Cosigners.Length) throw new FormatException();
            Script = reader.ReadVarBytes(ushort.MaxValue);
            if (Script.Length == 0) throw new FormatException();
        }

        public bool Equals(Transaction other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Hash.Equals(other.Hash);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Transaction);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public UInt160[] GetScriptHashesForVerifying()
        {
            var hashes = new HashSet<UInt160> { Sender };
            hashes.UnionWith(Cosigners.Select(p => p.Account));
            return hashes.OrderBy(p => p).ToArray();
        }

        void Serialize(BinaryWriter writer)
        {
            SerializeUnsigned(writer);
            writer.Write(Witnesses);
        }

        void SerializeUnsigned(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(Nonce);
            writer.Write(Sender);
            writer.Write(SystemFee);
            writer.Write(NetworkFee);
            writer.Write(ValidUntilBlock);
            writer.Write(Attributes);
            writer.Write(Cosigners);
            writer.WriteVarBytes(Script);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["hash"] = Hash.ToString();
            json["size"] = Size;
            json["version"] = Version;
            json["nonce"] = Nonce;
            json["sender"] = Sender.ScriptHash2Address();
            json["sys_fee"] = SystemFee.ToString();
            json["net_fee"] = NetworkFee.ToString();
            json["valid_until_block"] = ValidUntilBlock;
            json["attributes"] =new JValue( Attributes.Select(p => p.ToJson()).ToArray() );
            json["cosigners"] =new JValue( Cosigners.Select(p => p.ToJson()).ToArray() );
            json["script"] = Script.Bytes2HexString();
            json["witnesses"] =new JValue( Witnesses.Select(p => p.ToJson()).ToArray() );
            return json;
        }

        public static Transaction FromJson(JObject json)
        {
            Transaction tx = new Transaction();
            tx.Version = byte.Parse(json["version"].ToString());
            tx.Nonce = uint.Parse(json["nonce"].ToString());
            tx.Sender = json["sender"].ToString().Address2ScriptHash();
            tx.SystemFee = long.Parse(json["sys_fee"].ToString());
            tx.NetworkFee = long.Parse(json["net_fee"].ToString());
            tx.ValidUntilBlock = uint.Parse(json["valid_until_block"].ToString());
            tx.Attributes = ((JArray)json["attributes"]).Select(p => TransactionAttribute.FromJson((JObject)p)).ToArray();
            tx.Cosigners = ((JArray)json["cosigners"]).Select(p => Cosigner.FromJson((JObject)p)).ToArray();
            tx.Script = json["script"].ToString().HexString2Bytes();
            tx.Witnesses = ((JArray)json["witnesses"]).Select(p => Witness.FromJson((JObject)p)).ToArray();
            return tx;
        }

        public byte[] GetMessage()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((uint)1951352142);
                SerializeUnsigned(writer);
                writer.Flush();
                return ms.ToArray();
            }
        }

        public byte[] GetRawData()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                Serialize(writer);
                writer.Flush();
                return ms.ToArray();
            }
        }

        public void AddWitness(byte[] signdata, byte[] pubkey, string addrs)
        {
            {//额外的验证
                byte[] msg = null;
                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    SerializeUnsigned(writer);
                    writer.Flush();
                    msg = ms.ToArray();
                }

                var addr = Conversion.PublicKey2Address(pubkey);
                if (addr != addrs)
                    throw new Exception("wrong script");
            }

            var vscript = Conversion.PublicKey2AddressScript(pubkey);

            var sb = new ThinSdk.Neo.VM.ScriptBuilder();
            sb.EmitPush(signdata);
            var iscript = sb.ToArray();

            AddWitnessScript(vscript, iscript);
        }

        public void AddWitnessScript(byte[] vscript,byte[] iscript,UInt160 scriptHash = null)
        {
            var scripthash = Conversion.CalcHash160(vscript);
            List<Witness> wit = null;
            if (Witnesses == null)
            {
                wit = new List<Witness>();
            }
            else
            {
                wit = new List<Witness>(Witnesses);
            }
            Witness newwit = new Witness();
            newwit.VerificationScript = vscript;
            newwit.InvocationScript = iscript;
            foreach (var w in wit)
            {
                if (w.Address == newwit.Address)
                    throw new Exception("alread have this witness");
            }

            wit.Add(newwit);
            wit.Sort((a, b) =>
            {
                return a.ScriptHash.CompareTo(b.ScriptHash);
            });
            Witnesses = wit.ToArray();

        }

        /*
        public byte[] Sign(byte[] message, byte[] prikey, byte[] pubkey)
        {
            using (var ecdsa = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = prikey,
                Q = new ECPoint
                {
                    X = pubkey.Take(32).ToArray(),
                    Y = pubkey.Skip(32).ToArray()
                }
            }))
            {
                return ecdsa.SignData(message, HashAlgorithmName.SHA256);
            }
        }

        public bool VerifyData(byte[] message, byte[] signature, byte[] pubkey)
        {
            using (var ecdsa = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint
                {
                    X = pubkey.Take(32).ToArray(),
                    Y = pubkey.Skip(32).ToArray()
                }
            }))
            {
                return ecdsa.VerifyData(message, signature, HashAlgorithmName.SHA256);
            }
        }
        */
    }
}
