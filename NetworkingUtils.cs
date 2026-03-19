using Kitchen;
using Kitchen.NetworkSupport;
using System.Linq;

namespace KitchenCountUp
{
    public static class NetworkingUtils
    {
        public static bool IsHost()
        {
            if (Session.NetworkPeers == null) return true;

            bool isHost = Session.NetworkPeers.Count <= 1;
            if (!isHost)
            {
                var peers = Session.NetworkPeers.ToArray();
                foreach (var peer in peers)
                {
                    if (peer.Item2.Connection == ConnectionType.Local)
                    {
                        isHost = peer.Item1 == Session.HostIdentifier;
                        break;
                    }
                }
            }
            return isHost;
        }
    }
}
