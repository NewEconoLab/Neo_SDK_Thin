﻿
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ThinNeo.Cryptography;

namespace ThinNeo.NEP6
{
    public class NEP6Wallet
    {

        private readonly string path;
        public readonly ScryptParameters scrypt;
        public readonly Dictionary<string, NEP6Account> accounts;

        public NEP6Wallet(string path)
        {
            this.accounts = new Dictionary<string, NEP6Account>();

            this.path = path;
            if (File.Exists(path))
            {
                string txt = System.IO.File.ReadAllText(path);
                JObject wallet = JObject.Parse(txt);

                this.scrypt = ScryptParameters.FromJson(wallet["scrypt"] as JObject);
                var accounts = wallet["accounts"] as JArray;
                foreach (JObject a in accounts)
                {
                    var ac = NEP6Account.FromJson(a, this);
                    this.accounts[Helper.Bytes2HexString(ac.ScriptHash)] = ac;
                }
            }
            else
            {
                this.scrypt = ScryptParameters.Default;
            }
        }

        private void AddAccount(NEP6Account account)
        {
            accounts[Helper.Bytes2HexString(account.ScriptHash)] = account;
        }


        public NEP6Account CreateAccount(byte[] privateKey, string password)
        {
            var pubkey = Helper_NEO.GetPublicKey_FromPrivateKey(privateKey);
            NEP6Contract contract = new NEP6Contract
            {
                Script = Helper_NEO.GetAddressScript_FromPublicKey(pubkey)
            };
            var scripthash = Helper_NEO.CalcHash160(pubkey);

            var nep2key = Helper_NEO.GetNep2FromPrivateKey(privateKey, password);

            NEP6Account account = new NEP6Account(scripthash, nep2key)
            {
                Contract = contract
            };
            AddAccount(account);

            return account;
        }

        public void Save()
        {
            JObject wallet = new JObject();
            wallet["name"] = null;
            wallet["version"] = "1.0";
            wallet["scrypt"] = scrypt.ToJson();
            wallet["accounts"] = new JArray();
            foreach (var ac in accounts.Values)
            {
                var jnot = ac.ToJson();
                (wallet["accounts"] as JArray).Add(jnot);
            }
            wallet["extra"] = null;
            File.WriteAllText(path, wallet.ToString());
        }

    }
}
