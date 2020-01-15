using Newtonsoft.Json.Linq;
using System;
using System.IO;
using ThinSdk.Neo.IO;

namespace ThinSdk.Neo
{ 
    public class TransactionAttribute : ISerializable
    {
        public TransactionAttributeUsage Usage;
        public byte[] Data;

        public int Size => sizeof(TransactionAttributeUsage) + Data.GetVarSize();

        void ISerializable.Deserialize(BinaryReader reader)
        {
            Usage = (TransactionAttributeUsage)reader.ReadByte();
            if (!Enum.IsDefined(typeof(TransactionAttributeUsage), Usage))
                throw new FormatException();
            Data = reader.ReadVarBytes(252);
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Usage);
            writer.WriteVarBytes(Data);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["usage"] = new JValue(Usage);
            json["data"] = Data.Bytes2HexString();
            return json;
        }

        public static TransactionAttribute FromJson(JObject json)
        {
            TransactionAttribute transactionAttribute = new TransactionAttribute();
            transactionAttribute.Usage = (TransactionAttributeUsage)(byte.Parse(json["usage"].ToString()));
            transactionAttribute.Data = json["data"].ToString().HexString2Bytes();
            return transactionAttribute;
        }
    }
}
