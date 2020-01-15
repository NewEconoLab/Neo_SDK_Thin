using ThinSdk.Neo;
using ThinSdk.Neo.VM;

namespace ThinSdk.Token
{
    public class NEO : BaseToken
    {
        public NEO(ScriptBuilder _scriptBuilder) : base(new UInt160("0x9bde8f209c88dd0e7ca3bf0af0f476cdd8207789"), _scriptBuilder)
        {

        }
    }
}
