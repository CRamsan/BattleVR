﻿using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manager class to handle the Main Menu UI.
/// </summary>
public class MainMenuUIManager : MonoBehaviour
{
    public enum MENUS
    {
        MAINMENU, LOCALGAMES, SELECTLEVEL, CONFIRMATION, NONE
    }

    public GameObject StartGameMenu;
    public GameObject JoinGameMenu;
    public GameObject SelectLevelMenu;
    public GameObject ConfirmationMenu;

    private bool isInitialized = false;

    private void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;

        StartGameMenu.SetActive(true);
        StartGameMenu.transform.position = Vector3.zero;
        JoinGameMenu.SetActive(false);
        JoinGameMenu.transform.position = Vector3.zero;
        SelectLevelMenu.SetActive(false);
        SelectLevelMenu.transform.position = Vector3.zero;
        ConfirmationMenu.SetActive(false);
        ConfirmationMenu.transform.position = Vector3.zero;
    }

    // Use this for initialization
    void Start()
    {
        Setup();
    }

    public void SetActiveMenu(MENUS selectedMenu)
    {
        StartGameMenu.SetActive(selectedMenu == MENUS.MAINMENU);
        JoinGameMenu.SetActive(selectedMenu == MENUS.LOCALGAMES);
        SelectLevelMenu.SetActive(selectedMenu == MENUS.SELECTLEVEL);
        ConfirmationMenu.SetActive(selectedMenu == MENUS.CONFIRMATION);
    }
}
