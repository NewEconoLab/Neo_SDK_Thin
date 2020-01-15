using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using ThinSdk.NET;

namespace sdk.UniTest
{
    public class UnitHttpHelper
    {
        [Test]
        public async Task Test_RpcPost()
        {
            var result =  await HttpHelper.RpcPost("http://localhost:20332", "getblock",new JValue(1));
            var count = JObject.Parse(result)["result"];
            Assert.IsNotNull(count);
        }

        [Test]
        public async Task Test_RpcGet()
        {
            var result = await HttpHelper.RpcGet("http://localhost:20332", "getblock", new JValue(1));
            var count = JObject.Parse(result)["result"];
            Assert.IsNotNull(count);
        }
    }
}
