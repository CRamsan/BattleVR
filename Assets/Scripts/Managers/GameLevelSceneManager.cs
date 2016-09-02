﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// This manager will take care of several kinds of UI and network related code. There should only be one instance of
/// this script for each Game Level Scene. 
/// </summary>
public class GameLevelSceneManager : MonoBehaviour, GameLevelUIManagerDelegate
{
    public enum TEAMTAG { NONE, RED, BLUE };

    //This should be a reference to the ONLY instance of this scripts
    public static GameLevelSceneManager instance;

    private GameObject capitalShipBlue;
    private GameObject capitalShipRed;

    private GameLevelUIManager uiManager;
    private ExtendedNetworkManager networkManager;
    private GameLevelSceneManagerDelegate sceneManagerDelegate;
    private NetworkDiscovery networkDiscovery;

    //
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
            sceneManagerDelegate.OnPauseMenuResumeSelected();
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
            ExtendedNetworkManager.isHost = false;
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
            sceneManagerDelegate.OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG.BLUE);
        }
    }

    public void OnTeamSelectMenuRedSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG.RED);
        }
    }

    public void OnShipConfigMenuFigtherSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnShipConfigMenuShipSelected(ShipController.ShipType.FIGHTER);
        }
    }

    public void OnShipConfigMenuFrigateSelected()
    {
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnShipConfigMenuShipSelected(ShipController.ShipType.FRIGATE);
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
        return (capitalShip.transform.position) + (capitalShip.transform.forward * 150);
    }
}
