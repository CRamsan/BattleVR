using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

public class GameLevelSceneManager : MonoBehaviour {

    public GameLevelUIManager uiManager;

    private ExtendedNetworkManager networkManager;
    private GameLevelSceneManagerDelegate sceneManagerDelegate;

    // Use this for initialization
    void Start()
    {
        networkManager = (ExtendedNetworkManager)NetworkManager.singleton;
    }

    public void SetDelegate(GameLevelSceneManagerDelegate sceneManagerDelegate)
    {
        this.sceneManagerDelegate = sceneManagerDelegate;
    }

    public void PauseMenuResume()
    {
        Assert.IsNotNull(sceneManagerDelegate);
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnPauseMenuDismissed();
        }
    }

    public void HideAllMenus()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.NONE);
    }

    public void DisplayPauseMenu()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.PAUSEMENU);
    }

    public void DisplayConfirmation()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.CONFIRMATION);
    }

    public void QuitGame()
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

    public void DisplayShipSelectMenu()
    {
        uiManager.SetActiveMenu(GameLevelUIManager.MENUS.SHIPSELECT);
    }

    public void SelectFighterShip()
    {
        Assert.IsNotNull(sceneManagerDelegate);
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnShipConfigMenuDismissed();
        }
    }

    public void SelectFrigateShip()
    {
        Assert.IsNotNull(sceneManagerDelegate);
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnShipConfigMenuDismissed();
        }
    }

    public void SelectTeamRed()
    {
        Assert.IsNotNull(sceneManagerDelegate);
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnTeamSelectMenuDismissed(LevelSceneManager.TEAMTAG.RED);
        }
    }

    public void SelectTeamBlue()
    {
        Assert.IsNotNull(sceneManagerDelegate);
        if (sceneManagerDelegate != null)
        {
            sceneManagerDelegate.OnTeamSelectMenuDismissed(LevelSceneManager.TEAMTAG.BLUE);
        }
    }
}
