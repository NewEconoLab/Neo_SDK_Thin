﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using ThinSdk.Neo.Cryptography.ECC;
using ThinSdk.Neo.IO;

namespace ThinSdk.Neo
{
    public class Cosigner : ISerializable
    {
        public UInt160 Account;
        public WitnessScope Scopes;
        public UInt160[] AllowedContracts;
        public ECPoint[] AllowedGroups;

        public Cosigner()
        {
            this.Scopes = WitnessScope.Global;
        }

        // This limits maximum number of AllowedContracts or AllowedGroups here
        private int MaxSubitems = 16;

        public int Size =>
            /*Account*/             UInt160.Length +
            /*Scopes*/              sizeof(WitnessScope) +
            /*AllowedContracts*/    (Scopes.HasFlag(WitnessScope.CustomContracts) ? AllowedContracts.GetVarSize() : 0) +
            /*AllowedGroups*/       (Scopes.HasFlag(WitnessScope.CustomGroups) ? AllowedGroups.GetVarSize() : 0);

        void ISerializable.Deserialize(BinaryReader reader)
        {
            Account = reader.ReadSerializable<UInt160>();
            Scopes = (WitnessScope)reader.ReadByte();
            AllowedContracts = Scopes.HasFlag(WitnessScope.CustomContracts)
                ? reader.ReadSerializableArray<UInt160>(MaxSubitems)
                : new UInt160[0];
            AllowedGroups = Scopes.HasFlag(WitnessScope.CustomGroups)
                ? reader.ReadSerializableArray<ECPoint>(MaxSubitems)
                : new ECPoint[0];
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write(Account);
            writer.Write((byte)Scopes);
            if (Scopes.HasFlag(WitnessScope.CustomContracts))
                writer.Write(AllowedContracts);
            if (Scopes.HasFlag(WitnessScope.CustomGroups))
                writer.Write(AllowedGroups);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["account"] = Account.ToString();
            json["scopes"] =new JValue(Scopes);
            if (Scopes.HasFlag(WitnessScope.CustomContracts))
                json["allowedContracts"] = new JValue( AllowedContracts.Select(p => (JObject)p.ToString()).ToArray() );
            if (Scopes.HasFlag(WitnessScope.CustomGroups))
                json["allowedGroups"] =new JValue( AllowedGroups.Select(p => (JObject)p.ToString()).ToArray() );
            return json;
        }

        public static Cosigner FromJson(JObject json)
        {
            return new Cosigner
            {
                Account = UInt160.Parse(json["account"].ToString()),
                Scopes = (WitnessScope)Enum.Parse(typeof(WitnessScope), json["scopes"].ToString()),
                AllowedContracts = ((JArray)json["allowedContracts"])?.Select(p => UInt160.Parse(p.ToString())).ToArray(),
                AllowedGroups = ((JArray)json["allowedGroups"])?.Select(p => ECPoint.Parse(p.ToString(), ECCurve.Secp256r1)).ToArray()
            };
        }
    }
}
