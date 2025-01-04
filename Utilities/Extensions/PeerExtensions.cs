using Marscore.Connection;
using Marscore.P2P.Peer;

namespace Marscore.Utilities.Extensions
{
    public static class PeerExtensions
    {
        public static bool IsWhitelisted(this INetworkPeer peer)
        {
            return peer.Behavior<IConnectionManagerBehavior>()?.Whitelisted == true;
        }
    }
}
