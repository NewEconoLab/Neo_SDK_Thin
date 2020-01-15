using ThinSdk.Neo.Cryptography;
using ThinSdk.Neo.Cryptography.ECC;
using ThinSdk.Neo.IO;
using ThinSdk.Neo.IO.Json;

namespace ThinSdk.Neo.SmartContract.Manifest
{
    /// <summary>
    /// A group represents a set of mutually trusted contracts. A contract will trust and allow any contract in the same group to invoke it, and the user interface will not give any warnings.
    /// A group is identified by a public key and must be accompanied by a signature for the contract hash to prove that the contract is indeed included in the group.
    /// </summary>
    public class ContractGroup
    {
        /// <summary>
        /// Pubkey represents the public key of the group.
        /// </summary>
        public ECPoint PubKey { get; set; }

        /// <summary>
        /// Signature is the signature of the contract hash.
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// Parse ContractManifestGroup from json
        /// </summary>
        /// <param name="json">Json</param>
        /// <returns>Return ContractManifestGroup</returns>
        public static ContractGroup FromJson(JObject json)
        {
            return new ContractGroup
            {
                PubKey = ECPoint.Parse(json["pubKey"].AsString(), ECCurve.Secp256r1),
                Signature = json["signature"].AsString().HexString2Bytes(),
            };
        }

        /// <summary>
        /// Return true if the signature is valid
        /// </summary>
        /// <param name="hash">Contract Hash</param>
        /// <returns>Return true or false</returns>
        public bool IsValid(UInt160 hash)
        {
            return Crypto.Default.VerifySignature(hash.ToArray(), Signature, PubKey.EncodePoint(false));
        }

        public virtual JObject ToJson()
        {
            var json = new JObject();
            json["pubKey"] = PubKey.ToString();
            json["signature"] = Signature.Bytes2HexString();
            return json;
        }
    }
}
