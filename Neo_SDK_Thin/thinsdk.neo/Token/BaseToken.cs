using System.Numerics;
using ThinSdk.Neo;
using ThinSdk.Neo.VM;

namespace ThinSdk.Token
{
    public class BaseToken: Contract
    {
        public BaseToken(UInt160 _contractHash, ScriptBuilder _scriptBuilder) : base(_contractHash,_scriptBuilder)
        {
        }

        public void Transfer(UInt160 from,UInt160 to, BigInteger amount)
        {
            Call("transfer", from, to, amount);
            ScriptBuilder.Emit(OpCode.THROWIFNOT);
        }

        public void Deploy()
        {
            Call("deploy");
        }

        public void Call_TotalSupply()
        {
            Call("totalSupply");
        }

        public void Call_BalanceOf(params UInt160[] accounts)
        {
            foreach (UInt160 account in accounts)
            {
                Call("balanceOf", account);
            }
        }

        public void Call_BalanceOf_Unite(params UInt160[] accounts)
        {
            ScriptBuilder.EmitPush(0);
            foreach (UInt160 account in accounts)
            {
                Call("balanceOf", account);
                ScriptBuilder.Emit(OpCode.ADD);
            }
        }

        public void Call_Decimals()
        {
            Call("decimals");
        }

        public void Call_Name()
        {
            Call("name");
        }
    }
}
