using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This controller implements the NetworkDiscoveryDelegate and can be set as the delegate for the ExtendedNetworkDiscoveryController.
/// When the ExtendedNetworkDiscoveryController find a new game then OnReceivedBroadcast will be called.
/// </summary>
public class DiscoveredGamesController : MonoBehaviour
{

    public GameObject uiLayout;
    public GameObject buttonPrefab;
    public GameObject waitingLabel;
    public MainMenuUIManager uiManager;
    public ExtendedNetworkDiscoveryController networkDiscovery;

    private ArrayList roomList;
    private bool isInitialized = false;

    void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;
        roomList = new ArrayList();
        Assert.IsNotNull(uiManager);
        Assert.IsNotNull(networkDiscovery);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        ExtendedNetworkDiscoveryController.OnGameFound += OnReceivedBroadcast;
        //Refresh the UI every 5 seconds starting 5 seconds from now
        InvokeRepeating("RefreshUI", 5, 5);
        networkDiscovery.StartAsClient();
    }

    void OnDisable()
    {
        networkDiscovery.StopBroadcast();
        CancelInvoke("RefreshUI");
        ExtendedNetworkDiscoveryController.OnGameFound -= OnReceivedBroadcast;
    }

    /// <summary>
    /// This method should be called when a new game is discovered.
    /// </summary>
    /// <param name="fromAddress">Address from the game discovered</param>
    /// <param name="data">Extra data, currently not used</param>
    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        HandleBroadcast(fromAddress, data);
    }

    /// <summary>
    /// Internal method to handle a new game being discovered. This method will check against the list of already found games.
    /// If the address does not exist, it will be added.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
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

    /// <summary>
    /// Private method to set the NetworkManager to the provided address.
    /// </summary>
    /// <param name="fromAddress"></param>
    private void SetNetworkManagerAddress(string fromAddress)
    {
        NetworkManager.singleton.networkAddress = fromAddress;
        uiManager.DisplayJoinConfirmation();
    }

    /// <summary>
    /// This method will refresh the UI with the games found. 
    /// </summary>
    private void RefreshUI()
    {
        foreach (Transform child in uiLayout.transform)
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
                roomButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SetNetworkManagerAddress(address);
                });
                roomButton.transform.SetParent(uiLayout.transform);
            }
        }
        else
        {
            waitingLabel.SetActive(true);
        }
    }
}
