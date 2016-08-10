using UnityEngine.Networking;

/// <summary>
/// This script will extend the NetworkManager to provide extra functionality.
/// </summary>
public class ExtendedNetworkManager : NetworkManager
{
    /// <summary>
    /// Provide a reference to the NetworkDiscovery
    /// </summary>
    public ExtendedNetworkDiscoveryController networkDiscovery;
    /// <summary>
    /// Bool to know if this instance belongs to the host of the multiplayer game
    /// </summary>
    public static bool isHost;
}
