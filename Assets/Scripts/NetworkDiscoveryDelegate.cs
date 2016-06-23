using System;

public interface NetworkDiscoveryDelegate
{
    void OnReceivedBroadcast(string fromAddress, string data);
}
