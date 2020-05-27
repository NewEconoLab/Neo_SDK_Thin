using NUnit.Framework;
using System.Threading.Tasks;
using ThinSdk;
using ThinSdk.Neo.SmartContract;
using ThinSdk.Neo.SmartContract.Manifest;
using System;
using System.Numerics;
using Newtonsoft.Json.Linq;
using ThinSdk.Neo;
using System.Text;
using ThinSdk.Neo.VM;
using ThinSdk.Token;
using NUnit.Framework.Constraints;
using ThinSdk.NET;
using System.IO;

namespace sdk.UniTest
{
    public class UnitNeoTransaction
    {
        [Test]
        public async Task Test_Transfer()
        {
            var b = Convert.FromBase64String("AgCj4REMFM5hb390YX4PxLgFWDryYCojjfY/DBRk306+kjNNH8fmS/HR4zlC2ZBeURPADAh0cmFuc2ZlcgwUO303EcbwzPmx3KkD0b+h2JbxI4xBYn1bUjg=");
            string str = b.Bytes2HexString();
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var addr2 = Conversion.Address2ScriptHash("NejD7DJWzD48ZG4gXKDVZt3QLf1fpNe1PF");
            var cli = new ThinSdk.NET.CLI("http://47.99.35.147:20332");
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY"),count,cli);
            nt.gas.Transfer(addr1,addr2, 300000000);
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
            Assert.IsNotNull(r);
        }
        [Test]
        public async Task Test_Invoke()
        {
            var addr = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var sb = new ScriptBuilder();
            var bt = new BaseToken(new UInt160("0x8c23f196d8a1bfd103a9dcb1f9ccf0c611377d3b"), sb);
            bt.Call_BalanceOf(addr);
            var bt2 = new BaseToken(new UInt160("0x9bde8f209c88dd0e7ca3bf0af0f476cdd8207789"),sb);
            bt.Call_BalanceOf(addr);
            var cli = new ThinSdk.NET.CLI();
            var r = await cli.GetInvokeData(sb.ToArray().Bytes2HexString());
            Assert.IsNotNull(r);
        }

        [Test]
        public async Task Test_CreateContract()
        {
            //var b = Convert.FromBase64String("AwC4ZNlFAAAADBTOYW9/dGF+D8S4BVg68mAqI432PwwUZN9OvpIzTR/H5kvx0eM5QtmQXlETwAwIdHJhbnNmZXIMFDt9NxHG8Mz5sdypA9G/odiW8SOMQWJ9W1I4"); //I3bWcD6Jri7\u002BCSo2fPuihpfMNmo=
            //var b2 = new UInt160(b);
            //var bb = System.Text.Encoding.UTF8.GetString(b);
            var addr1 = Conversion.Address2ScriptHash("NejD7DJWzD48ZG4gXKDVZt3QLf1fpNe1PF");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(addr1,count, cli);
            NefFile nefFile = NefFile.LoadNef("test.nef");
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            nt.InitContract(nefFile, contractManifest).Create();
            var r = await nt.Send(Conversion.WIF2PrivateKey("L2JHYBNdCwuBqP4HKkqA1VwdZwNAjGgJXFbHP6wa7rEQkaTbWZ2V"));
            Assert.IsNotNull(r);
        }

        [Test]
        public async Task Test_InvokeNep5Contract_Name()
        {
            uint api = BitConverter.ToUInt32(ThinSdk.Neo.Cryptography.Helper.Sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes("System.Contract.Call")), 0);
            byte[] a =BitConverter.GetBytes(api);
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(addr1, count, cli);
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            var contractHash = contractManifest.Hash;
            nt.InitNep5(contractHash).Call_Name();
            var data = (JObject.Parse(await nt.Invoke())["result"]["stack"] as JArray)[0]["value"].ToString();
            byte[] b = System.Convert.FromBase64String(data);
            var name = System.Text.Encoding.UTF8.GetString(b);
            Assert.IsNotNull(name);
        }

        [Test]
        public async Task Test_InvokeNep5Contract_TotalSupply()
        {
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(addr1, count, cli);
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            var contractHash = contractManifest.Hash;
            nt.InitNep5(contractHash).Call_TotalSupply();
            var data = (JObject.Parse(await nt.Invoke())["result"]["stack"] as JArray)[0];
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task Test_InvokeNep5Contract_Deploy()
        {
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY"), count, cli);
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            var contractHash = contractManifest.Hash;
            nt.InitNep5(contractHash).Deploy();
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
            Assert.IsNotNull(r);
        }


        [Test]
        public async Task Test_InvokeNep5Contract_Transfer()
        {
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var addr2 = Conversion.Address2ScriptHash("NejD7DJWzD48ZG4gXKDVZt3QLf1fpNe1PF");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY"), count, cli);
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            var contractHash = contractManifest.Hash;


            nt.InitNep5(contractHash).Transfer(addr1,addr2,100000000);
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
            Assert.IsNotNull(r);
        }
    }
}
