using System.Linq;
using ThinSdk.Neo.IO.Json;

namespace ThinSdk.Neo.SmartContract.Manifest
{
    /// <summary>
    /// For technical details of ABI, please refer to NEP-3: NeoContract ABI. (https://github.com/neo-project/proposals/blob/master/nep-3.mediawiki)
    /// </summary>
    public class ContractAbi
    {
        /// <summary>
        /// Hash is the script hash of the contract. It is encoded as a hexadecimal string in big-endian.
        /// </summary>
        public UInt160 Hash { get; set; }

        /// <summary>
        /// Entrypoint is a Method object which describe the details of the entrypoint of the contract.
        /// </summary>
        public ContractMethodDescriptor EntryPoint { get; set; }

        /// <summary>
        /// Methods is an array of Method objects which describe the details of each method in the contract.
        /// </summary>
        public ContractMethodDescriptor[] Methods { get; set; }

        /// <summary>
        /// Events is an array of Event objects which describe the details of each event in the contract.
        /// </summary>
        public ContractEventDescriptor[] Events { get; set; }

        /// <summary>
        /// Parse ContractAbi from json
        /// </summary>
        /// <param name="json">Json</param>
        /// <returns>Return ContractAbi</returns>
        public static ContractAbi FromJson(JObject json)
        {
            return new ContractAbi
            {
                Hash = UInt160.Parse(json["hash"].AsString()),
                EntryPoint = ContractMethodDescriptor.FromJson(json["entryPoint"]),
                Methods = ((JArray)json["methods"]).Select(u => ContractMethodDescriptor.FromJson(u)).ToArray(),
                Events = ((JArray)json["events"]).Select(u => ContractEventDescriptor.FromJson(u)).ToArray()
            };
        }

        public JObject ToJson()
        {
            var json = new JObject();
            json["hash"] = Hash.ToString();
            json["entryPoint"] = EntryPoint.ToJson();
            json["methods"] = new JArray(Methods.Select(u => u.ToJson()).ToArray());
            json["events"] = new JArray(Events.Select(u => u.ToJson()).ToArray());
            return json;
        }
    }
}
