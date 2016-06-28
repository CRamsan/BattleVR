using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class GameLevelUIManager : MonoBehaviour {

    public enum MENUS
    {
        PAUSEMENU, CONFIRMATION, NONE
    }

    public GameObject PauseGameMenu;
    public GameObject ConfirmationMenu;
    public Vector3 canvasPosition;

    private bool isInitialized = false;

    private void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;

        PauseGameMenu.transform.position = canvasPosition;
        ConfirmationMenu.transform.position = canvasPosition;
    }

    // Use this for initialization
    void Start()
    {
        Setup();
    }

    public void SetActiveMenu(MENUS selectedMenu)
    {
        PauseGameMenu.SetActive(selectedMenu == MENUS.PAUSEMENU);
        ConfirmationMenu.SetActive(selectedMenu == MENUS.CONFIRMATION);
    }
}
