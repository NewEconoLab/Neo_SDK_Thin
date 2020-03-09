using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Numerics;
using System;

namespace ThinSdk.NET
{
    public class CLI
    {
        public string URL = "";

        public CLI(string _url = "http://localhost:10332")  //localhost
        {
            URL = _url;
        }

        public async Task<string> Sendrawtransaction(string hexStr)
        {
            var result = await HttpHelper.RpcPost(URL,"sendrawtransaction",new JValue(hexStr));
            return result;
        }

        public async Task<uint> GetBlockCount()
        {
            var result = await HttpHelper.RpcPost(URL, "getblockcount");
            var count = (uint)JObject.Parse(result)?["result"];
            return count;
        }

        public async Task<string> GetInvokeData(string hexStr)
        {
            var result = await HttpHelper.RpcPost(URL, "invokescript", new JValue(hexStr));
            return result;
        }

        public async Task<BigInteger> GetInvokeGasConsumed(string hexStr)
        {
            var result = await HttpHelper.RpcPost(URL, "invokescript", new JValue(hexStr));
            var gas_consumed =decimal.Parse((string)JObject.Parse(result)?["result"]["gas_consumed"]);
            return BigInteger.Parse((gas_consumed).ToString());
        }
    }
}
