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

    private bool isInitialized = false;

    private void Setup()
    {
        Assert.IsFalse(isInitialized);
        isInitialized = true;

        PauseGameMenu.SetActive(true);
        PauseGameMenu.transform.position = Vector3.zero;
        ConfirmationMenu.SetActive(false);
        ConfirmationMenu.transform.position = Vector3.zero;
    }

    // Use this for initialization
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetActiveMenu(MENUS selectedMenu)
    {
        PauseGameMenu.SetActive(selectedMenu == MENUS.PAUSEMENU);
        ConfirmationMenu.SetActive(selectedMenu == MENUS.CONFIRMATION);
    }
}
