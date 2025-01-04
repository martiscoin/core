using System.IO;

namespace Marscore.NBitcoin.BouncyCastle.asn1
{
    internal interface Asn1OctetStringParser
        : IAsn1Convertible
    {
        Stream GetOctetStream();
    }
}
