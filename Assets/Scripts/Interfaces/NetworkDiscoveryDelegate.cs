/// <summary>
/// This delegate defines the method that will be called when the NetworkDiscovery discovers a new game.
/// </summary>
public interface NetworkDiscoveryDelegate
{
    /// <summary>
    /// Implement this method to handle a new game being discovered in the local network.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    void OnReceivedBroadcast(string fromAddress, string data);
}
