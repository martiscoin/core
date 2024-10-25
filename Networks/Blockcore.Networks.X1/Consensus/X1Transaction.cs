using Blockcore.Consensus.TransactionInfo;

namespace Blockcore.Networks.X1.Consensus
{
    public class X1Transaction : Transaction
    {
        public override bool IsProtocolTransaction()
        {
            return this.IsCoinBase || this.IsCoinStake;
        }

        public byte[] LotHeaderBytes { get; set; }

        public uint LotNonce { get; set; }
        public ulong LotFoundHeight { get; set; }
    }
}