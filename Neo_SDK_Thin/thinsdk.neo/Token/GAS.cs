using ThinSdk.Neo;
using ThinSdk.Neo.VM;

namespace ThinSdk.Token
{
    public class GAS : BaseToken
    {
        public GAS(ScriptBuilder _scriptBuilder) : base(new UInt160("0x8c23f196d8a1bfd103a9dcb1f9ccf0c611377d3b"), _scriptBuilder)
        {

        }
    }
}
