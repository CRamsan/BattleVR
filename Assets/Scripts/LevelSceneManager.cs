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
            GameObject aiPlayer = Instantiate(aiPrefab);
            //aiPlayer.transform.position = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
            switch (i)
            {
                case 0:
                    aiPlayer.transform.position = new Vector3(100, 0, 0);
                    break;
                case 1:
                    aiPlayer.transform.position = new Vector3(0, 100, 0);
                    break;
                case 2:
                    aiPlayer.transform.position = new Vector3(0, 0, 100);
                    break;
                default:
                    break;
            }
            NetworkServer.Spawn(aiPlayer);
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
