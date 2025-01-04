using System.Linq;
using Marscore.P2P;
using Marscore.P2P.Peer;

namespace Marscore.Utilities.Extensions
{
    public static class NodeConnectionParameterExtensions
    {
        public static PeerAddressManagerBehaviour PeerAddressManagerBehaviour(this NetworkPeerConnectionParameters parameters)
        {
            return parameters.TemplateBehaviors.OfType<PeerAddressManagerBehaviour>().FirstOrDefault();
        }
    }
}