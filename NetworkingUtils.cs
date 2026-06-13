using Kitchen;
using Kitchen.NetworkSupport;
using Platforms;
using System.Linq;

namespace KitchenCountUp
{
    public static class NetworkingUtils
    {
        public static bool IsHost()
        {
            return Session.NetworkedPlayState != NetworkedPlayState.Client;
        }
    }
}
