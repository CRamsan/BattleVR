using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.UI;

public class DiscoveredGamesController : MonoBehaviour, NetworkDiscoveryDelegate {

    private ExtendedNetworkManager networkManager;
    private ExtendedNetworkDiscovery networkDiscovery;
    private ArrayList roomList;

    public GameObject buttonPrefab;
    public GameObject layout;
    public GameObject waitingLabel;
    public MainMenuSceneManager menuManager;

    private bool isInitialized = false;

    void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;
        networkManager = (ExtendedNetworkManager)NetworkManager.singleton;
        networkDiscovery = networkManager.networkDiscovery;
        roomList = new ArrayList();
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
	
	}

    void Stop()
    {
    }

    void OnEnable()
    {
        if (!isInitialized)
        {
            Setup();
        }

        networkDiscovery.Initialize();
        networkDiscovery.discoveryDelegate = this;
        InvokeRepeating("RefreshUI", 5, 5);
        networkDiscovery.StartAsClient();
    }

    void OnDisable()
    {
        CancelInvoke("RefreshUI");
        networkDiscovery.StopBroadcast();
        networkManager.networkDiscovery.discoveryDelegate = null;
    }

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        HandleBroadcast(fromAddress, data);
    }

    private void HandleBroadcast(string fromAddress, string data)
    {
        bool addressFound = false;
        foreach (string address in roomList)
        {
            addressFound = address == fromAddress;
            if (addressFound)
            {
                break;
            }
        }
        if (!addressFound)
        {
            roomList.Add(fromAddress);
        }
    }

    private void SetNetworkManagerAddress(string fromAddress)
    {
        networkManager.networkAddress = fromAddress;
        menuManager.DisplayJoinConfirmation();
    }

    private void RefreshUI()
    {
        foreach (Transform child in layout.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (roomList.Count > 0)
        {
            waitingLabel.SetActive(false);
            foreach (string address in roomList)
            {
                GameObject roomButton = Instantiate(buttonPrefab);
                roomButton.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                roomButton.GetComponentsInChildren<Text>()[0].text = address;
                roomButton.GetComponent<Button>().onClick.AddListener(() => {
                    SetNetworkManagerAddress(address);
                    });
                roomButton.transform.SetParent(layout.transform);
            }
        }
        else
        {
            waitingLabel.SetActive(true);
        }
    }
}
