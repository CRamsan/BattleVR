using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using System.Collections;

public class LevelSceneManager : MonoBehaviour {

    public GameObject capitalShipBlue;
    public GameObject capitalShipRed;

    public enum TEAMTAG { RED, BLUE };

    private ExtendedNetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    // Use this for initialization
    void Start () {
        networkManager = (ExtendedNetworkManager)NetworkManager.singleton;
        networkDiscovery = networkManager.networkDiscovery;
        SetGameVisibility(true);
	}

    void OnDestroy()
    {
        SetGameVisibility(false);
    }

    // Update is called once per frame
    void Update () {
	
	}

    private void SetGameVisibility(bool visible)
    {
        if (visible)
        {
            networkDiscovery.Initialize();
            networkDiscovery.StartAsServer();
        }
        else
        {
            networkDiscovery.StopBroadcast();
        }
    }
}
