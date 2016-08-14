using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This controller will provide the input management for movement and UI for an agent controlled by a human player.
/// </summary>
public class PlayerController : ShipController, GameLevelSceneManagerDelegate {

    public GameObject pauseCanvasPrefab;
    public GameObject shipConfigCanvasPrefab;

    private Collider playerCollider;
    private GameLevelSceneManager sceneManager;
    private GameObject canvasGameObject;
    private bool isPause;
    private bool isTeamSelected;

    // Use this for initialization
    void Start()
    {
        Init();
        if (isLocalPlayer)
        {
            isPause = false;
            isTeamSelected = false;
            sceneManager = GameLevelSceneManager.instance;
            playerCollider = GetComponent<SphereCollider>();
            DisplayShipConfigMenu();
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (child.name == "MainPlayerCamera")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }

        bool pausePressed = InputManager.WasActionPressed(InputManager.CONTROLLER_ACTION.PAUSE);

        if (canvasGameObject != null)
        {
            canvasGameObject.transform.position = transform.TransformPoint(new Vector3(0, 1, 5));
            canvasGameObject.transform.rotation = transform.rotation;
        }

        if (pausePressed)
        {
            TogglePauseMenu();
            return;
        }

        if (isPause)
        {
            return;
        }

        float dStrafe = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.STRAFE);
        float dForward = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.THRUSTER);
        float dLookUp = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.LOOK_UP);
        float dLookSide = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.LOOK_SIDE);
        float dRotate = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.ROTATE);

        bool fireTrigger = InputManager.IsActionPressed(InputManager.CONTROLLER_ACTION.SHOOT_PRIMARY);
        if (fireTrigger)
        {
            gunController.PressTriger();
        }

        Vector3 leftStickVector = new Vector3(dStrafe, 0, dForward);
        Vector3 rotationStickVector = new Vector3(dLookUp, dLookSide, dRotate);

        HandleInput(leftStickVector.magnitude >= 0.2 ? leftStickVector : Vector3.zero,
                    rotationStickVector.magnitude >= 0.2 ? rotationStickVector : Vector3.zero);
    }

    /// <summary>
    /// Private method to display the configuration menu. During the first call this method will display the menu
    /// to select a team. Afterwards the menu to select a ship will be displayed. This method does not toggle the
    /// menu so the menu should be dismissed with DismissShipConfigMenu.
    /// </summary>
    private void DisplayShipConfigMenu()
    {
        playerCollider.enabled = false;
        gameRenderer.enabled = false;
        canvasGameObject = Instantiate(shipConfigCanvasPrefab);
        sceneManager.SetUIManager(canvasGameObject.GetComponent<GameLevelUIManager>());
        sceneManager.SetDelegate(this);
        if (!isTeamSelected)
        {
            sceneManager.DisplayTeamSelectMenu();
        }
        else
        {
            sceneManager.DisplayShipSelectMenu();
        }
    }

    /// <summary>
    /// This method will dismiss the menu displayed by DisplayShipConfigMenu.
    /// </summary>
    private void DismissShipConfigMenu()
    {
        sceneManager.HideAllMenus();
        Destroy(canvasGameObject);
        canvasGameObject = null;
        playerCollider.enabled = true;
        gameRenderer.enabled = true;
    }

    /// <summary>
    /// This method will be called when the user selects a team. Here the player should be configured to match the team settings.
    /// </summary>
    /// <param name="teamTag"></param>
    public void OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG teamTag)
    {
        setTeam(teamTag);
        isTeamSelected = true;
        sceneManager.DisplayShipSelectMenu();
    }

    /// <summary>
    /// This method will be called when the user selected a ship from the UI. This method will set the variables 
    /// and spawn the player in the correct position.
    /// </summary>
    public void OnShipConfigMenuShipSelected(ShipController.ShipType type)
    {
        setShipType(type);
        DismissShipConfigMenu();
        transform.position = Vector3.zero;
    }

    /// <summary>
    /// Internal method to hide or display the pause menu.
    /// </summary>
    private void TogglePauseMenu()
    {
        if (isPause)
        {
            sceneManager.HideAllMenus();
            Destroy(canvasGameObject);
            canvasGameObject = null;
        }
        else
        {
            canvasGameObject = Instantiate(pauseCanvasPrefab);
            sceneManager.SetUIManager(canvasGameObject.GetComponent<GameLevelUIManager>());
            sceneManager.SetDelegate(this);
            sceneManager.DisplayPauseMenu();
        }
        isPause = !isPause;
    }

    /// <summary>
    /// This method will be called when the player selected the "Resume" 
    /// </summary>
    public void OnPauseMenuResumeSelected()
    {
        Assert.IsTrue(isPause);
        Assert.IsNotNull(canvasGameObject);
        TogglePauseMenu();
    }
}
