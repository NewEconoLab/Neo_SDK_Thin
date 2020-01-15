using ThinSdk.Neo;
using ThinSdk.Neo.SmartContract;
using ThinSdk.Neo.SmartContract.Manifest;
using ThinSdk.Neo.VM;
using ThinSdk.Token;

namespace ThinSdk
{
    public class Contract
    {
        public UInt160 ContractHash;

        public byte[] Script;

        public ContractManifest Manifest;

        public ScriptBuilder ScriptBuilder;

        public Contract(UInt160 _contractHash,ScriptBuilder _scriptBuilder)
        {
            ContractHash = _contractHash;
            ScriptBuilder = _scriptBuilder;
        }

        public Contract(NefFile _nefFile, ContractManifest _contractManifest, ScriptBuilder _scriptBuilder) 
        {
            ContractHash = _nefFile.ScriptHash;
            Script = _nefFile.Script;
            Manifest = _contractManifest;
            ScriptBuilder = _scriptBuilder;
        }

        public void Deploy()
        {
            if (Script == null || Script.Length == 0)
                throw new System.Exception();
            ScriptBuilder.EmitSysCall("System.Contract.Create", Script,Manifest.ToJson().ToString());
        }

        public void Call(string method,params object[] args)
        {
            ScriptBuilder.EmitAppCall(ContractHash,method,args);
        }
    }
}
