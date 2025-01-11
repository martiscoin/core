using System.Threading;
using Martiscoin.Consensus;
using Martiscoin.Consensus.Chain;

namespace Martiscoin.Interfaces
{
    public interface IBlockStoreQueue : IBlockStore
    {
        void ReindexChain(IConsensusManager consensusManager, CancellationToken nodeCancellation);

        /// <summary>Adds a block to the saving queue.</summary>
        /// <param name="chainedHeaderBlock">The block and its chained header pair to be added to pending storage.</param>
        void AddToPending(ChainedHeaderBlock chainedHeaderBlock);

        /// <summary>The highest stored block in the block store cache or <c>null</c> if block store feature is not enabled.</summary>
        ChainedHeader BlockStoreCacheTip { get; }
    }
}
