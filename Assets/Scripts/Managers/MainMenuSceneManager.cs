﻿using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This manager will provide high level UI and network actions for the MainMenu scene.
/// </summary>
public class MainMenuSceneManager : MonoBehaviour
{
    public MainMenuUIManager uiManager;

    private NetworkManager networkManager;
    private bool inJoinFlow = false;

    // Use this for initialization
    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Methods that will be called by the buttons in the MainMenu

    public void DisplayMainMenu()
    {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.MAINMENU);
    }

    public void DisplayLocalGame()
    {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.LOCALGAMES);
    }

    public void CancelStartGame()
    {
        if (!inJoinFlow)
        {
            DisplaySelectLevel();
        }
        else
        {
            DisplayLocalGame();
        }
    }

    public void DisplaySelectLevel()
    {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.SELECTLEVEL);
    }

    public void DisplayConfirmation()
    {
        inJoinFlow = false;
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.CONFIRMATION);
    }

    public void DisplayJoinConfirmation()
    {
        inJoinFlow = true;
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.CONFIRMATION);
    }

    public void DisplaySettings() { }

    public void DisplayCredits() { }

    public void ConfirmStartGame()
    {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.NONE);
        if (inJoinFlow)
        {
            networkManager.StartClient();
        }
        else
        {
            networkManager.StartHost();
        }
        ExtendedNetworkManager.isHost = !inJoinFlow;
    }
}