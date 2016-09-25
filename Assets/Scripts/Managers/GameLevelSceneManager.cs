using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// This manager will take care of several kinds of UI and network related code. There should only be one instance of
/// this script for each Game Level Scene. 
/// </summary>
public class GameLevelSceneManager : MonoBehaviour
{
    public enum TEAMTAG { NONE, RED, BLUE };

    //This should be a reference to the ONLY instance of this scripts
    public static GameLevelSceneManager instance;

    private GameObject capitalShipBlue;
    private GameObject capitalShipRed;

    private GameLevelUIManager uiManager;
    private ExtendedNetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    //
    public void SetUIManager(GameLevelUIManager uiManager)
    {
        Assert.IsNotNull(uiManager);
        this.uiManager = uiManager;
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

    // Use this for initialization
    void Start()
    {
        GameLevelSceneManager.instance = this;
        networkManager = (ExtendedNetworkManager)NetworkManager.singleton;
        networkDiscovery = networkManager.networkDiscovery;
        SetGameVisibility(true);
    }

    void OnDestroy()
    {
        SetGameVisibility(false);
        GameLevelSceneManager.instance = null;
    }

    public void HideAllMenus()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.NONE);
        uiManager = null;
    }

    public void DisplayPauseMenu()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.PAUSEMENU);
    }

    public void DisplayShipSelectMenu()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.SHIPSELECT);
    }

    public void DisplayTeamSelectMenu()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.TEAMSELECT);
    }

    public void DisplayGameEndMenu()
    {
        GameLevelEventManager.PauseMenuConfirmQuitSelectedEvent += QuitGame;
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.GAMEEND);
    }

    public void QuitGame()
    {
        GameLevelEventManager.PauseMenuConfirmQuitSelectedEvent -= QuitGame;
        if (ExtendedNetworkManager.isHost)
        {
            ExtendedNetworkManager.isHost = false;
            networkManager.StopHost();
        }
        else
        {
            networkManager.StopClient();
        }
    }

    /// <summary>
    /// Set the target gameObject as the capital ship of the specified team.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="target"></param>
    public void RegisterCapitalShip(TEAMTAG tag, GameObject target)
    {
        switch (tag)
        {
            case TEAMTAG.RED:
                capitalShipRed = target;
                break;
            case TEAMTAG.BLUE:
                capitalShipBlue = target;
                break;
            default:
                throw new UnityException();
        }
    }

    public void OnCapitalShipDestroyed(TEAMTAG tag)
    {
        GameObject winner;
        GameObject loser;
        switch (tag)
        {
            case TEAMTAG.RED:
                loser = capitalShipRed;
                winner = capitalShipBlue;
                break;
            case TEAMTAG.BLUE:
                loser = capitalShipBlue;
                winner = capitalShipRed;
                break;
            default:
                throw new UnityException();
        }
        winner.GetComponent<TeamController>().OnGameEnded(true);
        loser.GetComponent<TeamController>().OnGameEnded(false);
    }

    /// <summary>
    /// Returns a Vector3 that should be used as the spawn point for the next ship. The spawn location
    /// will be based on the location of the game objects registered as capital ship for each team;
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public Vector3 GetSpawnPosition(TEAMTAG tag)
    {
        GameObject capitalShip;
        switch (tag)
        {
            case TEAMTAG.RED:
                capitalShip = capitalShipRed;
                break;
            case TEAMTAG.BLUE:
                capitalShip = capitalShipBlue;
                break;
            default:
                throw new UnityException();
        }

        // This is just a hack to get spawn points in a spherical shape around the capital ship
        return (capitalShip.transform.position) + (Vector3.up * 100);
    }

    public TeamController GetTeamController(TEAMTAG tag)
    {
        GameObject capitalShip;
        switch (tag)
        {
            case TEAMTAG.RED:
                capitalShip = capitalShipRed;
                break;
            case TEAMTAG.BLUE:
                capitalShip = capitalShipBlue;
                break;
            default:
                throw new UnityException();
        }

        TeamController controller = capitalShip.GetComponent<TeamController>();
        Assert.IsNotNull(controller);
        return controller;
    }
}
