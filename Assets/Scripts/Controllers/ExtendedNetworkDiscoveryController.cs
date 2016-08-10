using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// This manager will recieve notifications when a new game is discovered in the local network and 
/// then the OnReceivedBroadcast method will be called in the discoveryDelegate.
/// </summary>
public class ExtendedNetworkDiscoveryController : NetworkDiscovery
{

    /// <summary>
    /// Public delegate that will be called whenever OnReceivedBroadcast is called.
    /// </summary>
    public NetworkDiscoveryDelegate discoveryDelegate;

    /// <summary>
    /// This method is called whenever a new game is discovered.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
#if UNITY_EDITOR
        Assert.IsNotNull(discoveryDelegate);
        if (discoveryDelegate == null)
        {
            return;
        }
#endif
        discoveryDelegate.OnReceivedBroadcast(fromAddress, data);
    }
}
