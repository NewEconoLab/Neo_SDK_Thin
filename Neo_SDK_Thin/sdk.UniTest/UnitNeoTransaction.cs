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

namespace sdk.UniTest
{
    public class UnitNeoTransaction
    {
        [Test]
        public async Task Test_Transfer()
        {
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var addr2 = Conversion.Address2ScriptHash("NejD7DJWzD48ZG4gXKDVZt3QLf1fpNe1PF");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY"),count,cli);
            nt.neo.Transfer(addr1, addr2, 200);
            nt.gas.Transfer(addr1,addr2,30000000000);
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
            Assert.IsNotNull(r);
        }

        [Test]
        public async Task Test_CreateContract()
        {
            var b = Convert.FromBase64String("I3bWcD6Jri7+CSo2fPuihpfMNmo="); //I3bWcD6Jri7\u002BCSo2fPuihpfMNmo=
            var b2 = new UInt160(b);
            var bb = System.Text.Encoding.UTF8.GetString(b);
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(addr1,count, cli);
            NefFile nefFile = NefFile.LoadNef("test.nef");
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            nt.InitContract(nefFile, contractManifest).Create();
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
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
