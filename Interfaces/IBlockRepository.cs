using System.Threading.Tasks;
using Martiscoin.Consensus.BlockInfo;
using Martiscoin.NBitcoin;

namespace Martiscoin.Interfaces
{
    public interface INBitcoinBlockRepository
    {
        Task<Block> GetBlockAsync(uint256 blockId);
    }

    public interface IBlockTransactionMapStore
    {
        uint256 GetBlockHash(uint256 trxHash);
    }
}
