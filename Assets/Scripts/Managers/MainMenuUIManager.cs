using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

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

        StartGameMenu.transform.position = Vector3.zero;
        JoinGameMenu.transform.position = Vector3.zero;
        SelectLevelMenu.transform.position = Vector3.zero;
        ConfirmationMenu.transform.position = Vector3.zero;
        SetActiveMenu(MENUS.MAINMENU);
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

        //TODO I am using this hack to set the first selected item after changing
        // menus. There must be a better way to do this.
        try
        {
            GameObject nextButton = GetComponentInChildren<Button>().gameObject;
            EventSystem es = GetComponentInChildren<EventSystem>();
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(nextButton);
        }
        catch
        { }
    }
}
