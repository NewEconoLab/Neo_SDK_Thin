using Newtonsoft.Json.Linq;
using System.Linq;

namespace ThinNeo.NEP6
{
    public class NEP6Contract
    {
        public byte[] Script;
        public static NEP6Contract FromJson(Newtonsoft.Json.Linq.JObject json)
        {
            if (json == null) return null;
            return new NEP6Contract
            {
                Script = Helper.HexString2Bytes((json["script"] as Newtonsoft.Json.Linq.JValue).Value as string),
            };
        }

        public Newtonsoft.Json.Linq.JObject ToJson()
        {
            Newtonsoft.Json.Linq.JObject contract = new Newtonsoft.Json.Linq.JObject();
            contract["script"] =  Helper.Bytes2HexString(Script);
            contract["parameters"] = new JArray();

            {
                JObject item = new JObject();
                item["name"] = "signature";
                item["type"] = "Signature";
                (contract["parameters"] as JArray).Add(item);
            }
            contract["deployed"] = false;
            return contract;
        }
    }
}
