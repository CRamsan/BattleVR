using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ExtendedNetworkManager : NetworkManager
{
    public ExtendedNetworkDiscoveryController networkDiscovery;
    public static bool isHost;
}
