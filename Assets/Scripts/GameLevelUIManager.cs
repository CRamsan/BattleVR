using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class GameLevelUIManager : MonoBehaviour {

    public enum MENUS
    {
        PAUSEMENU, CONFIRMATION, SHIPSELECT, TEAMSELECT, NONE
    }

    public GameObject PauseGameMenu;
    public GameObject ConfirmationMenu;
    public GameObject ShipSelectMenu;
    public GameObject TeamSelectMenu;
    public Vector3 canvasPosition;

    private bool isInitialized = false;

    private void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;

        PauseGameMenu.transform.localPosition = canvasPosition;
        ConfirmationMenu.transform.localPosition = canvasPosition;
        ShipSelectMenu.transform.localPosition = canvasPosition;
        TeamSelectMenu.transform.localPosition = canvasPosition;
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
        if (ShipSelectMenu)  ShipSelectMenu.SetActive(selectedMenu == MENUS.SHIPSELECT);
        if (TeamSelectMenu) TeamSelectMenu.SetActive(selectedMenu == MENUS.TEAMSELECT);
    }
}
