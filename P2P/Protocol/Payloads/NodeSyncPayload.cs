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
        string nodeid;
        string[] ips;

        public string NodeID { get { return this.nodeid; } set { this.nodeid = value; } }
        public string[] IPs { get { return this.ips; } set { this.ips = value; } }

        public NodeSyncPayload(string nodeid, string[] ips)
        {
            this.nodeid = nodeid;
            this.ips = ips;
        }

        public NodeSyncPayload()
        {
        }

        public override void ReadWriteCore(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.nodeid);
            stream.ReadWrite(ref this.ips);
        }

        public override string ToString()
        {
            return base.ToString() + " : " + this.nodeid + "," + this.ips;
        }

    }

    public class NodeInfo
    {
        string nodeid;
        string[] ips;
        DateTime lstupdatetime;

        public string NodeID { get { return this.nodeid; } set { this.nodeid = value; } }
        public string[] IP { get { return this.ips; } set { this.ips = value; } }
        public DateTime LstUpdateTime { get { return this.lstupdatetime; } set { this.lstupdatetime = value; } }

        public NodeInfo(string nodeid, string[] ips)
        {
            this.nodeid = nodeid;
            this.ips = ips;
            this.lstupdatetime = DateTime.UtcNow;
        }
    }
}
