using NUnit.Framework;
using ThinSdk;
namespace sdk.UniTest
{
    public class UnitConversion
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_GetAddressFromPublicKey()
        {
            var priKey = Conversion.WIF2PrivateKey("L1EboNBetFw1JRdoQCjFDjApLrg3pHu62VrD6B983exKYzYpJc1e");
            var pubKey = Conversion.PrivateKey2PublicKey(priKey);
            var str_pub = Conversion.Bytes2HexString(pubKey);
            var assress = Conversion.PublicKey2Address(pubKey);
            Assert.AreEqual(assress, "AdyoCmmAvg8tQcC4KbM4G8FpfW5ANHWpvN");
        }
    }
}