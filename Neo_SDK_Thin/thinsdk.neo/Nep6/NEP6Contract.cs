using Newtonsoft.Json.Linq;

namespace ThinSdk.NEP6
{
    public class NEP6Contract
    {
        public byte[] Script;
        public static NEP6Contract FromJson(Newtonsoft.Json.Linq.JObject json)
        {
            if (json == null) return null;
            return new NEP6Contract
            {
                Script = Conversion.HexString2Bytes((json["script"] as JValue).Value as string),
            };
        }

        public JObject ToJson()
        {
            JObject contract = new JObject();
            contract["script"] = Conversion.Bytes2HexString(Script);
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
