using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

public class GameLevelSceneManager : MonoBehaviour, GameLevelUIManagerDelegate
{

    public static GameLevelSceneManager instance;

    private GameLevelUIManager uiManager;
    private ExtendedNetworkManager networkManager;
    private GameLevelSceneManagerDelegate sceneManagerDelegate;

    public GameObject capitalShipBlue;
    public GameObject capitalShipRed;

    public enum TEAMTAG { RED, BLUE };

    private NetworkDiscovery networkDiscovery;

    public void SetUIManager(GameLevelUIManager uiManager)
    {
        Assert.IsNotNull(uiManager);
        this.uiManager = uiManager;
        this.uiManager.uiManagerDelegate = this;
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

    public void SetDelegate(GameLevelSceneManagerDelegate sceneManagerDelegate)
    {
        this.sceneManagerDelegate = sceneManagerDelegate;
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


    public void OnPauseMenuResumeSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnPauseMenuDismissed();
        }
    }

    public void OnPauseMenuQuitSelected()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.CONFIRMATION);
    }

    public void OnPauseMenuConfirmBackSelected()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.PAUSEMENU);
    }

    public void OnPauseMenuConfirmQuitSelected()
    {
        if (ExtendedNetworkManager.isHost)
        {
            networkManager.StopHost();
        }
        else
        {
            networkManager.StopClient();
        }
    }

    public void OnTeamSelectMenuBlueSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnTeamSelectMenuDismissed(LevelSceneManager.TEAMTAG.BLUE);
        }
    }

    public void OnTeamSelectMenuRedSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnTeamSelectMenuDismissed(LevelSceneManager.TEAMTAG.RED);
        }
    }

    public void OnShipConfigMenuFigtherSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnShipConfigMenuDismissed();
        }
    }

    public void OnShipConfigMenuFrigateSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnShipConfigMenuDismissed();
        }
    }
}
