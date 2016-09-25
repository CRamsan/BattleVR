using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manager class to handle the UI during in-game. This script needs to have a uiManagerDelegate to delegate the actions.
/// </summary>
public class GameLevelUIManager : MonoBehaviour
{

    public enum MENUS
    {
        PAUSEMENU, CONFIRMATION, SHIPSELECT, TEAMSELECT, GAMEEND, NONE
    }

    public GameObject PauseGameMenu;
    public GameObject ConfirmationMenu;
    public GameObject ShipSelectMenu;
    public GameObject TeamSelectMenu;
    public GameObject GameEndMenu;

    private bool isInitialized = false;

    private void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;

        if (PauseGameMenu) PauseGameMenu.transform.localPosition = Vector3.zero;
        if (ConfirmationMenu) ConfirmationMenu.transform.localPosition = Vector3.zero;
        if (ShipSelectMenu) ShipSelectMenu.transform.localPosition = Vector3.zero;
        if (TeamSelectMenu) TeamSelectMenu.transform.localPosition = Vector3.zero;
        if (GameEndMenu) GameEndMenu.transform.localPosition = Vector3.zero;
    }

    // Use this for initialization
    void Start()
    {
        Setup();
    }

    public void SetActiveMenu(MENUS selectedMenu)
    {
        if (PauseGameMenu) PauseGameMenu.SetActive(selectedMenu == MENUS.PAUSEMENU);
        if (ConfirmationMenu) ConfirmationMenu.SetActive(selectedMenu == MENUS.CONFIRMATION);
        if (ShipSelectMenu) ShipSelectMenu.SetActive(selectedMenu == MENUS.SHIPSELECT);
        if (TeamSelectMenu) TeamSelectMenu.SetActive(selectedMenu == MENUS.TEAMSELECT);
        if (GameEndMenu) GameEndMenu.SetActive(selectedMenu == MENUS.GAMEEND);

        try
        {
            if (MENUS.NONE != selectedMenu)
            {
                GameObject nextButton = GetComponentInChildren<Button>().gameObject;
                EventSystem es = GetComponentInChildren<EventSystem>();
                es.SetSelectedGameObject(null);
                es.SetSelectedGameObject(nextButton);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void PauseMenuResume()
    {
        GameLevelEventManager.OnPauseMenuResumeSelected();
    }

    public void PauseMenuQuit()
    {
        SetActiveMenu(GameLevelUIManager.MENUS.CONFIRMATION);
    }

    public void PauseMenuConfirmBack()
    {
        SetActiveMenu(GameLevelUIManager.MENUS.PAUSEMENU);
    }

    public void PauseMenuConfirmQuit()
    {
        GameLevelEventManager.OnPauseMenuConfirmQuitSelected();
    }

    public void TeamSelectMenuBlue()
    {
        GameLevelEventManager.OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG.BLUE);
    }

    public void TeamSelectMenuRed()
    {
        GameLevelEventManager.OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG.RED);
    }

    public void ShipConfigMenuFigther()
    {
        GameLevelEventManager.OnShipConfigMenuShipSelected(ShipController.ShipType.FIGHTER);
    }

    public void ShipConfigMenuFrigate()
    {
        GameLevelEventManager.OnShipConfigMenuShipSelected(ShipController.ShipType.FRIGATE);
    }

    public void ShipConfigMenuDestroyer()
    {
        GameLevelEventManager.OnShipConfigMenuShipSelected(ShipController.ShipType.DESTROYER);
    }
}
