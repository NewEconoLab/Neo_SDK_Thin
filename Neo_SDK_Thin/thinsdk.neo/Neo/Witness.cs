using Newtonsoft.Json.Linq;
using System.IO;
using ThinSdk.Neo.IO;

namespace ThinSdk.Neo
{
    public class Witness : ISerializable
    {
        public byte[] InvocationScript;
        public byte[] VerificationScript;

        private UInt160 _scriptHash;
        public virtual UInt160 ScriptHash
        {
            get
            {
                if (_scriptHash == null)
                {
                    _scriptHash = Conversion.CalcHash160(VerificationScript);
                }
                return _scriptHash;
            }
        }
        public string Address { get { return Conversion.ScriptHash2Address(_scriptHash); } }

        public int Size => InvocationScript.GetVarSize() + VerificationScript.GetVarSize();

        void ISerializable.Deserialize(BinaryReader reader)
        {
            // This is designed to allow a MultiSig 10/10 (around 1003 bytes) ~1024 bytes
            // Invocation = 10 * 64 + 10 = 650 ~ 664  (exact is 653)
            InvocationScript = reader.ReadVarBytes(664);
            // Verification = 10 * 33 + 10 = 340 ~ 360   (exact is 350)
            VerificationScript = reader.ReadVarBytes(360);
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.WriteVarBytes(InvocationScript);
            writer.WriteVarBytes(VerificationScript);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["invocation"] = InvocationScript.Bytes2HexString();
            json["verification"] = VerificationScript.Bytes2HexString();
            return json;
        }

        public static Witness FromJson(JObject json)
        {
            Witness witness = new Witness();
            witness.InvocationScript = json["invocation"].ToString().HexString2Bytes();
            witness.VerificationScript = json["verification"].ToString().HexString2Bytes();
            return witness;
        }
    }
}
