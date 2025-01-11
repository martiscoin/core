using System.Collections.Generic;
using Martiscoin.Consensus.BlockInfo;
using Martiscoin.NBitcoin;
using Martiscoin.Networks;

namespace Martiscoin.Mining
{
    public sealed class BlockTemplate
    {
        public Block Block { get; set; }

        public Money TotalFee { get; set; }

        public Dictionary<uint256, Money> FeeDetails { get; set; }

        public BlockTemplate(Network network)
        {
            this.Block = network.CreateBlock();
        }
    }
}