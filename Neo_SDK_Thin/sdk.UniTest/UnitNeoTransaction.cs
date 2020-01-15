using NUnit.Framework;
using System.Threading.Tasks;
using ThinSdk;
using ThinSdk.Neo.SmartContract;
using ThinSdk.Neo.SmartContract.Manifest;
using System;
using System.Numerics;
using Newtonsoft.Json.Linq;

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
            nt.neo.Transfer(addr1, addr2, 2);
            nt.gas.Transfer(addr1,addr2,300000000);
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
            Assert.IsNotNull(r);
        }

        [Test]
        public async Task Test_DeployContract()
        {

            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var addr2 = Conversion.Address2ScriptHash("NejD7DJWzD48ZG4gXKDVZt3QLf1fpNe1PF");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY"), count, cli);
            NefFile nefFile = NefFile.LoadNef("test.nef");
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            nt.InitContract(nefFile, contractManifest).Deploy();
            var r = await nt.Send(Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e"));
            Assert.IsNotNull(r);
        }

        [Test]
        public async Task Test_InvokeNep5Contract()
        {
            var addr1 = Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY");
            var addr2 = Conversion.Address2ScriptHash("NejD7DJWzD48ZG4gXKDVZt3QLf1fpNe1PF");
            var cli = new ThinSdk.NET.CLI();
            var count = await cli.GetBlockCount();
            var nt = new NeoTransaction(Conversion.Address2ScriptHash("NV7LGd57KEsCw2YfDcRosQmeb2qvdamipY"), count, cli);
            ContractManifest contractManifest = ContractManifest.LoadContractManifest("test.manifest.json");
            var contractHash = contractManifest.Hash;
            nt.InitNep5(contractHash).Call_Name();
            var data = (JObject.Parse(await nt.Invoke())["result"]["stack"] as JArray)[0]["value"];
            byte[] b = System.Convert.FromBase64String("V29ybGQ=");
            var name = System.Text.Encoding.UTF8.GetString(b);
            Assert.AreEqual(name,"test3");
        }
    }
}
