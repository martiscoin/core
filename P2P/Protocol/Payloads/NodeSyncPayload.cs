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
        string ip;
        string ipv6;

        public string NodeID { get { return this.nodeid; } set { this.nodeid = value; } }
        public string IP { get { return this.ip; } set { this.ip = value; } }
        public string IPV6 { get { return this.ipv6; } set { this.ipv6 = value; } }

        public NodeSyncPayload(string nodeid, string ip,string ipv6)
        {
            this.nodeid = nodeid;
            this.ip = ip;
            this.ipv6 = ipv6;
        }

        public NodeSyncPayload()
        {
        }

        public override void ReadWriteCore(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.nodeid);
            stream.ReadWrite(ref this.ip);
            stream.ReadWrite(ref this.ipv6);
        }

        public override string ToString()
        {
            return base.ToString() + " : " + this.nodeid + "," + this.ip + "," + this.ipv6;
        }

    }

    public class NodeInfo
    {
        string nodeid;
        string ip;
        string ipv6;
        DateTime lstupdatetime;

        public string NodeID { get { return this.nodeid; } set { this.nodeid = value; } }
        public string IP { get { return this.ip; } set { this.ip = value; } }
        public string IPV6 { get { return this.ipv6; } set { this.ipv6 = value; } }
        public DateTime LstUpdateTime { get { return this.lstupdatetime; } set { this.lstupdatetime = value; } }

        public NodeInfo(string nodeid, string ip, string ipv6)
        {
            this.nodeid = nodeid;
            this.ip = ip;
            this.ipv6 = ipv6;
            this.lstupdatetime = DateTime.UtcNow;
        }
    }
}
