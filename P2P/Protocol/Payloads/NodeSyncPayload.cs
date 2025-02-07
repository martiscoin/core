using Martiscoin.Consensus.BlockInfo;
using Martiscoin.NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Martiscoin.P2P.Protocol.Payloads
{
    [Payload("nodesync")]
    public class NodeSyncPayload : Payload
    {
        string _nodes;

        /// <summary>
        /// current nodes inclued all NodeInfo
        /// </summary>
        public string nodes { get { return this._nodes; } set { this._nodes = value; } }

        public NodeSyncPayload(string nodes)
        {
            this._nodes = nodes;
        }

        public NodeSyncPayload()
        {
        }

        public override void ReadWriteCore(BitcoinStream stream)
        {
            stream.ReadWrite(ref this._nodes);
        }

        public override string ToString()
        {
            return base.ToString() + " : " + this.nodes;
        }

    }

    public class NodeInfo
    {
        string nodeid;//current node id
        DateTime lstupdatetime;//last update online time

        public string NodeID { get { return this.nodeid; } set { this.nodeid = value; } }
        public DateTime LstUpdateTime { get { return this.lstupdatetime; } set { this.lstupdatetime = value; } }

        public NodeInfo(string nodeid)
        {
            this.nodeid = nodeid;
            this.lstupdatetime = DateTime.UtcNow;
        }
    }
}
