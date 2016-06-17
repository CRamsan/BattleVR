using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MainMenuSceneManager : MonoBehaviour {

    public NetworkManager networkManager;
    public MainMenuUIManager uiManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DisplayMainMenu() {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.MAINMENU);
    }

    public void DisplayLocalGame() {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.LOCALGAMES);
    }

    public void DisplaySelectLevel()
    {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.SELECTLEVEL);
    }

    public void DisplayConfirmation()
    {
        uiManager.SetActiveMenu(MainMenuUIManager.MENUS.CONFIRMATION);
    }

    public void DisplaySettings() { }

    public void DisplayCredits() { }

    public void ConfirmStartGame() {
        networkManager.StartHost();
    }
}
