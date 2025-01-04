using Marscore.Networks;

namespace Marscore.NBitcoin
{
    public interface IBech32Data : IBitcoinString
    {
        Bech32Type Type
        {
            get;
        }
    }
}
