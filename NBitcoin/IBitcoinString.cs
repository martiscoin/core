using Martiscoin.Networks;

namespace Martiscoin.NBitcoin
{
    public interface IBitcoinString
    {
        Network Network
        {
            get;
        }
    }
}
