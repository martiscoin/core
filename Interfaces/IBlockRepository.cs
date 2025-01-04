using System.Threading.Tasks;
using Marscore.Consensus.BlockInfo;
using Marscore.NBitcoin;

namespace Marscore.Interfaces
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
