using Marscore.Consensus.BlockInfo;
using Marscore.NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marscore.P2P.Protocol.Payloads
{
    [Payload("storage")]
    public class StoragePayload : Payload
    {
        uint nonce;
        ulong height;
        string address;
        BlockHeader header;
        public uint LotNonce { get { return this.nonce; } set { this.nonce = value; } }
        public ulong LotFoundHeight { get { return this.height; } set { this.height = value; } }

        public string Address { get { return this.address; }set { this.address = value; } }

        public BlockHeader Header { get { return this.header; }set { this.header = value; } }

        public StoragePayload(uint nonce, ulong hegiht,string address,BlockHeader header)
        {
            this.height = hegiht;
            this.nonce = nonce;
            this.address = address;
            this.header = header;
        }

        public StoragePayload()
        {
        }

        public override void ReadWriteCore(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.nonce);
            stream.ReadWrite(ref this.height);
            stream.ReadWrite(ref this.address);
            stream.ReadWrite(ref this.header);
        }

        public override string ToString()
        {
            return base.ToString() + " : " + this.LotNonce + "," + this.LotFoundHeight;
        }
    }

    public class LotTransaction
    {
        public uint LotNonce { get; set; }
        public ulong LotFoundHeight { get; set; }
        public string Address { get; set; }

        public BlockHeader Header { get; set; }

        public LotTransaction() { }
    }
}
