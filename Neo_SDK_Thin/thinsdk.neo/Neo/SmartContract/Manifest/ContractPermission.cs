using System;
using System.Linq;
using ThinSdk.Neo.IO.Json;

namespace ThinSdk.Neo.SmartContract.Manifest
{
    /// <summary>
    /// The permissions field is an array containing a set of Permission objects. It describes which contracts may be invoked and which methods are called.
    /// </summary>
    public class ContractPermission
    {
        /// <summary>
        /// The contract field indicates the contract to be invoked. It can be a hash of a contract, a public key of a group, or a wildcard *.
        /// If it specifies a hash of a contract, then the contract will be invoked; If it specifies a public key of a group, then any contract in this group will be invoked; If it specifies a wildcard*, then any contract will be invoked.
        /// </summary>
        public ContractPermissionDescriptor Contract { get; set; }

        /// <summary>
        /// The methods field is an array containing a set of methods to be called. It can also be assigned with a wildcard *. If it is a wildcard *, then it means that any method can be called.
        /// If a contract invokes a contract or method that is not declared in the manifest at runtime, the invocation will fail.
        /// </summary>
        public WildcardContainer<string> Methods { get; set; }

        public static readonly ContractPermission DefaultPermission = new ContractPermission
        {
            Contract = ContractPermissionDescriptor.CreateWildcard(),
            Methods = WildcardContainer<string>.CreateWildcard()
        };

        /// <summary>
        /// Parse ContractPermission from json
        /// </summary>
        /// <param name="json">Json</param>
        /// <returns>Return ContractPermission</returns>
        public static ContractPermission FromJson(JObject json)
        {
            return new ContractPermission
            {
                Contract = ContractPermissionDescriptor.FromJson(json["contract"]),
                Methods = WildcardContainer<string>.FromJson(json["methods"], u => u.AsString()),
            };
        }

        /// <summary
        /// To json
        /// </summary>
        public JObject ToJson()
        {
            var json = new JObject();
            json["contract"] = Contract.ToJson();
            json["methods"] = Methods.ToJson();
            return json;
        }

        /// <summary>
        /// Return true if is allowed
        /// </summary>
        /// <param name="manifest">The manifest of which contract we are calling</param>
        /// <param name="method">Method</param>
        /// <returns>Return true or false</returns>
        public bool IsAllowed(ContractManifest manifest, string method)
        {
            if (Contract.IsHash)
            {
                if (!Contract.Hash.Equals(manifest.Hash)) return false;
            }
            else if (Contract.IsGroup)
            {
                if (manifest.Groups.All(p => !p.PubKey.Equals(Contract.Group))) return false;
            }
            return Methods.IsWildcard || Methods.Contains(method);
        }
    }
}
