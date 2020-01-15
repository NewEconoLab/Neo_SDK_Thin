using Newtonsoft.Json.Linq;
using System;

namespace ThinSdk.NEP6
{
    public class NEP6Account
    {
        public byte[] ScriptHash;
        public string nep2key;
        public NEP6Contract Contract;

        public NEP6Account(byte[] scriptHash, string nep2key = null)
        {
            this.ScriptHash = scriptHash;
            this.nep2key = nep2key;
        }

        public override string ToString()
        {
            return Conversion.ScriptHash2Address(this.ScriptHash) + " " + ((this.nep2key != null) ? "[have key]" : "[no key]");
        }


        public static NEP6Account FromJson(Newtonsoft.Json.Linq.JObject json, NEP6Wallet wallet)
        {
            var strAdd = (json["address"] as Newtonsoft.Json.Linq.JValue).Value as string;
            var pubkeyhash = Conversion.Address2ScriptHash(strAdd);
            string key = null;
            if (json.ContainsKey("key") && json["key"] != null)
                key = json["key"].Value<string>();
            var acc = new NEP6Account(pubkeyhash, key);
            if (json.ContainsKey("contract") && json["contract"] != null)
            {
                acc.Contract = NEP6Contract.FromJson(json["contract"] as JObject);
            }
            return acc;
        }


        public byte[] GetPrivate(ScryptParameters sp, string password)
        {
            if (nep2key == null) return null;
            return Conversion.NEP22PrivateKey(nep2key, password, sp.N, sp.R, sp.P);
        }

        public JObject ToJson()
        {
            JObject account = new JObject();
            byte[] shash = (ScriptHash);
            var addr = Conversion.ScriptHash2Address(shash);
            account["address"] = addr;
            account["label"] = null;
            account["isDefault"] = false;
            account["lock"] = false;
            account["key"] = nep2key;
            account["contract"] = ((NEP6Contract)Contract)?.ToJson();
            account["extra"] = null;
            return account;
        }

        public bool VerifyPassword(string password)
        {
            try
            {
                var prikey = Conversion.NEP22PrivateKey(nep2key, password);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
