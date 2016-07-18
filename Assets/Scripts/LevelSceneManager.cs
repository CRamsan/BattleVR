using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using System.Collections;

public class LevelSceneManager : MonoBehaviour {

    public GameObject aiPrefab;

    private ExtendedNetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    // Use this for initialization
    void Start () {
        networkManager = (ExtendedNetworkManager)NetworkManager.singleton;
        networkDiscovery = networkManager.networkDiscovery;
        SetGameVisibility(true);

        for (int i = 0; i < 3; i++)
        {
            NetworkServer.Spawn(aiPrefab);
        }
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
