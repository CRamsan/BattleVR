using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ExtendedNetworkDiscovery : NetworkDiscovery {

    public NetworkDiscoveryDelegate discoveryDelegate;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (discoveryDelegate != null)
            discoveryDelegate.OnReceivedBroadcast(fromAddress, data);
    }
}
