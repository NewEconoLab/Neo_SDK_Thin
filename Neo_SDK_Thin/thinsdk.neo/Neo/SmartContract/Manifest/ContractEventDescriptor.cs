using System.Linq;
using ThinSdk.Neo.IO.Json;

namespace ThinSdk.Neo.SmartContract.Manifest
{
    public class ContractEventDescriptor
    {
        /// <summary>
        /// Name is the name of the method, which can be any valid identifier.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameters is an array of Parameter objects which describe the details of each parameter in the method.
        /// </summary>
        public ContractParameterDefinition[] Parameters { get; set; }

        /// <summary>
        /// Parse ContractMethodDescription from json
        /// </summary>
        /// <param name="json">Json</param>
        /// <returns>Return ContractMethodDescription</returns>
        public static ContractMethodDescriptor FromJson(JObject json)
        {
            return new ContractMethodDescriptor
            {
                Name = json["name"].AsString(),
                Parameters = ((JArray)json["parameters"]).Select(u => ContractParameterDefinition.FromJson(u)).ToArray(),
            };
        }

        public virtual JObject ToJson()
        {
            var json = new JObject();
            json["name"] = Name;
            json["parameters"] = new JArray(Parameters.Select(u => u.ToJson()).ToArray());
            return json;
        }
    }
}
