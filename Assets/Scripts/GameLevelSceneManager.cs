using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameLevelSceneManager : MonoBehaviour {

    public GameLevelUIManager uiManager;

    private NetworkManager networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = NetworkManager.singleton;
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
}
