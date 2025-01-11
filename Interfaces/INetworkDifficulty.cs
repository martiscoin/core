using Martiscoin.NBitcoin;

namespace Martiscoin.Interfaces
{
    public interface INetworkDifficulty
    {
        Target GetNetworkDifficulty();
    }
}
