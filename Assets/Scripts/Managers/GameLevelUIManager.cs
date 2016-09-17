using UnityEngine;
using UnityEngine.Assertions;

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

    public GameLevelUIManagerDelegate uiManagerDelegate;

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
    }

    public void PauseMenuResume()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnPauseMenuResumeSelected();
        }
    }

    public void PauseMenuQuit()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnPauseMenuQuitSelected();
        }
    }

    public void PauseMenuConfirmBack()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnPauseMenuConfirmBackSelected();
        }
    }

    public void PauseMenuConfirmQuit()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnPauseMenuConfirmQuitSelected();
        }
    }

    public void TeamSelectMenuBlue()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnTeamSelectMenuBlueSelected();
        }
    }

    public void TeamSelectMenuRed()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnTeamSelectMenuRedSelected();
        }
    }

    public void ShipConfigMenuFigther()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnShipConfigMenuFigtherSelected();
        }
    }

    public void ShipConfigMenuFrigate()
    {
        if (uiManagerDelegate != null)
        {
            uiManagerDelegate.OnShipConfigMenuFrigateSelected();
        }
    }
}
