using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// This manager will recieve notifications when a new game is discovered in the local network and 
/// then the OnReceivedBroadcast method will be called in the discoveryDelegate.
/// </summary>
public class ExtendedNetworkDiscoveryController : NetworkDiscovery
{
    /// <summary>
    /// This delegate is called when a new game was discovered in the local network.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public delegate void GameFound(string fromAddress, string data);
    public static event GameFound OnGameFound;

    /// <summary>
    /// This method is called whenever a new game is discovered.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Assert.IsNotNull(OnGameFound);
        if (OnGameFound != null)
        {
            OnGameFound(fromAddress, data);
        }
    }
}
