using Martiscoin.Consensus.Chain;
using Martiscoin.NBitcoin;
using Newtonsoft.Json;

namespace Martiscoin.EventBus.CoreEvents
{
    /// <summary>
    /// Event that is executed when a block is connected to a consensus chain.
    /// </summary>
    /// <seealso cref="EventBase" />
    public class BlockConnected : EventBase
    {
        [JsonIgnore]
        public ChainedHeaderBlock ConnectedBlock { get; }

        public uint256 Hash { get; set; }

        public int Height { get; set; }

        public BlockConnected(ChainedHeaderBlock connectedBlock)
        {
            this.ConnectedBlock = connectedBlock;

            this.Hash = this.ConnectedBlock.ChainedHeader.HashBlock;

            this.Height = this.ConnectedBlock.ChainedHeader.Height;
        }
    }
}