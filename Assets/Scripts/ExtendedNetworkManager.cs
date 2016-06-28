using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ExtendedNetworkManager : NetworkManager
{
    public ExtendedNetworkDiscovery networkDiscovery;
    public static bool isHost;
}
