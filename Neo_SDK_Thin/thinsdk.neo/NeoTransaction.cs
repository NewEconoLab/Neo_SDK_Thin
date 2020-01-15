using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ThinSdk.Neo;
using ThinSdk.Neo.Cryptography;
using ThinSdk.Neo.IO;
using ThinSdk.Neo.SmartContract;
using ThinSdk.Neo.SmartContract.Manifest;
using ThinSdk.Neo.VM;
using ThinSdk.NET;
using ThinSdk.Token;

namespace ThinSdk
{
    public class NeoTransaction
    {
        private ScriptBuilder scriptBuilder;

        internal Transaction Tran;

        private CLI Cli;

        public GAS gas;

        public NEO neo;

        public NeoTransaction(UInt160 _sender,uint currentBlockIndex, CLI _cli)
        {
            scriptBuilder = new ScriptBuilder();
            gas = new GAS(scriptBuilder);
            neo = new NEO(scriptBuilder);
            Cli = _cli;
            Tran = new Transaction
            {
                Version = 0,
                Nonce = (UInt32)new Random().Next(),
                Sender = _sender,
                ValidUntilBlock = currentBlockIndex + Transaction.MaxValidUntilBlockIncrement - 1,
                Cosigners = new Cosigner[1] { new Cosigner() { Scopes = WitnessScope.CalledByEntry,Account = _sender} },
                Attributes = new TransactionAttribute[0]
            };
        }

        public BaseToken InitNep5(UInt160 _contractHash)
        {
            return new BaseToken(_contractHash,scriptBuilder);   
        }

        public Contract InitContract(UInt160 _contractHash) 
        {
            return new Contract(_contractHash,scriptBuilder);
        }

        public Contract InitContract(NefFile _nefFile, ContractManifest _contractManifest)
        {
            return new Contract(_nefFile, _contractManifest, scriptBuilder);
        }

        public async Task<string> Invoke(bool clearScriptBuilder = true)
        {
            string result = await Cli.GetInvokeData(scriptBuilder.ToArray().Bytes2HexString());
            if (clearScriptBuilder)
                scriptBuilder = new ScriptBuilder();
            return result;
        }

        public async Task<string> Send(byte[] priKey)
        {
            BigInteger sysFee = await Cli.GetInvokeGasConsumed(scriptBuilder.ToArray().Bytes2HexString());
            sysFee = (sysFee / 100000000 + 1) * 100000000;
            var hexStr = SignAndPack(priKey, sysFee);
            return await Cli.Sendrawtransaction(hexStr);
        }

        public string GetTranMessgae()
        {
            return Conversion.Bytes2HexString(Tran.GetMessage());
        }

        public string GetScriptHexString()
        {
            return Conversion.Bytes2HexString(scriptBuilder.ToArray());
        }

        private string SignAndPack(byte[] priKey, BigInteger _sysFee)
        {
            Tran.Script = scriptBuilder.ToArray();
            var pubKey = Conversion.PrivateKey2PublicKey(priKey);
            var address = Conversion.PublicKey2Address(pubKey);
            var witness_script = Conversion.PublicKey2AddressScript(pubKey);

            //计算网络费
            int size = Transaction.HeaderSize + Tran.Attributes.GetVarSize() + Tran.Cosigners.GetVarSize() + Tran.Script.GetVarSize() + ThinSdk.Neo.IO.Helper.GetVarSize(Tran.Cosigners.Length);
            if (witness_script.IsSignatureContract())
            {
                size += 67 + witness_script.GetVarSize();
                Tran.NetworkFee += ApplicationEngine.OpCodePrices[OpCode.PUSHDATA1] + ApplicationEngine.OpCodePrices[OpCode.PUSHNULL] + ApplicationEngine.OpCodePrices[OpCode.PUSHDATA1] + 0_01000000;
            }
            else if (witness_script.IsMultiSigContract(out int m, out int n))
            {
                int size_inv = 66 * m;
                size += ThinSdk.Neo.IO.Helper.GetVarSize(size_inv) + size_inv + witness_script.GetVarSize();
                Tran.NetworkFee += ApplicationEngine.OpCodePrices[OpCode.PUSHDATA1] * m;
                using (ScriptBuilder sb = new ScriptBuilder())
                    Tran.NetworkFee += ApplicationEngine.OpCodePrices[(OpCode)sb.EmitPush(m).ToArray()[0]];
                Tran.NetworkFee += ApplicationEngine.OpCodePrices[OpCode.PUSHDATA1] * n;
                using (ScriptBuilder sb = new ScriptBuilder())
                    Tran.NetworkFee += ApplicationEngine.OpCodePrices[(OpCode)sb.EmitPush(n).ToArray()[0]];
                Tran.NetworkFee += ApplicationEngine.OpCodePrices[OpCode.PUSHNULL] + 0_01000000 * n;
            }
            else
            {
                //We can support more contract types in the future.
            }
            Tran.NetworkFee += size * 1000L;

            Tran.SystemFee = (long)_sysFee;

            //签名
            byte[] signData = Crypto.Default.Sign(Tran.GetMessage(), priKey, Conversion.PrivateKey2PublicKey_EncodePointFalse(priKey).Skip(1).ToArray());
            var b = Crypto.Default.VerifySignature(Tran.GetMessage(), signData, Conversion.PrivateKey2PublicKey_EncodePointFalse(priKey).Skip(1).ToArray());
            if (!b)
                throw new Exception("sign error");
            Tran.AddWitness(signData, pubKey, address);

            return Conversion.Bytes2HexString(Tran.GetRawData());
        }

    }
}
