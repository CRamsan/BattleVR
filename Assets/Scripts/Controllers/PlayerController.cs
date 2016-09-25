using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// This controller will provide the input management for movement and UI for an agent controlled by a human player.
/// </summary>
public class PlayerController : ShipController {

    public GameObject pauseCanvasPrefab;
    public GameObject shipConfigCanvasPrefab;
    public AudioClip impactSound;
    public AudioClip projectileSound;
    public Vector3 canvasMenuPosition;
    public Vector3 canvasMenuScale;

    private GameLevelSceneManager sceneManager;
    private GameObject canvasGameObject;
    private bool isPause;
    private bool isTeamSelected;

    // Use this for initialization
    void Start()
    {
        SafeInit();
        if (isLocalPlayer)
        {
            isPause = false;
            isTeamSelected = false;
            isReadyForGame = false;
            sceneManager = GameLevelSceneManager.instance;
            DisplayShipConfigMenu();
        }
        else
        {
            //TODO Investigate this and lets make it compact
            foreach (Transform child in transform)
            {
                if (child.name == "MainPlayerCamera" || child.name == "HUD")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            if (isReadyForGame)
            {
                RefreshTeamState();
                RefreshShipType();
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
            canvasGameObject.transform.position = transform.TransformPoint(canvasMenuPosition);
            canvasGameObject.transform.localScale = canvasMenuScale;
            canvasGameObject.transform.rotation = transform.rotation;
        }

        if (pausePressed && !gameEnded)
        {
            TogglePauseMenu();
            return;
        }

        Vector3 leftStickVector = Vector3.zero, rotationStickVector = Vector3.zero;

        if (!isPause && !gameEnded)
        { 
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

            leftStickVector = new Vector3(dStrafe, 0, dForward);
            rotationStickVector = new Vector3(dLookUp, dLookSide, dRotate);
        }

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
        isPause = true;
        // This can be null and it is not an issue. LODGroup will not be set until the player selects a ship.
        if (gameLODGroup != null)
        {
            gameLODGroup.SetActive(false);
        }
        canvasGameObject = Instantiate(shipConfigCanvasPrefab);
        sceneManager.SetUIManager(canvasGameObject.GetComponent<GameLevelUIManager>());
        if (!isTeamSelected)
        {
            GameLevelEventManager.TeamSelectMenuTeamSelectedEvent += OnTeamSelectMenuTeamSelected;
            sceneManager.DisplayTeamSelectMenu();
        }
        else
        {
            GameLevelEventManager.ShipConfigMenuShipSelectedEvent += OnShipConfigMenuShipSelected;
            sceneManager.DisplayShipSelectMenu();
        }
    }

    /// <summary>
    /// This method will dismiss the menu displayed by DisplayShipConfigMenu.
    /// </summary>
    private void DismissShipConfigMenu()
    {
        isPause = true;
        sceneManager.HideAllMenus();
        Destroy(canvasGameObject);
        canvasGameObject = null;
    }

    /// <summary>
    /// This method will be called when the user selects a team. Here the player should be configured to match the team settings.
    /// </summary>
    /// <param name="teamTag"></param>
    public void OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG teamTag)
    {
        GameLevelEventManager.TeamSelectMenuTeamSelectedEvent -= OnTeamSelectMenuTeamSelected;
        CmdDoSetTeam(teamTag);
        isTeamSelected = true;
        TeamController controller = sceneManager.GetTeamController(teamTag);
        controller.RegisterUnit(gameObject);
        GameLevelEventManager.ShipConfigMenuShipSelectedEvent += OnShipConfigMenuShipSelected;
        sceneManager.DisplayShipSelectMenu();
    }

    /// <summary>
    /// This method will be called when the user selected a ship from the UI. This method will set the variables 
    /// and spawn the player in the correct position.
    /// </summary>
    public void OnShipConfigMenuShipSelected(ShipController.ShipType type)
    {
        GameLevelEventManager.ShipConfigMenuShipSelectedEvent -= OnShipConfigMenuShipSelected;
        CmdDoSetShipType(type);
        DismissShipConfigMenu();
        transform.position = sceneManager.GetSpawnPosition(teamTag);
        CmdDoSetReadyForGame(true);
        isPause = false;
    }

    /// <summary>
    /// Set the isReadyForGame variable on the server. We need to do this because SyncVars
    /// hooks only work from the server to the client. So this method will update the variable 
    /// in the server and then the hook will propagate the change to the clients.
    /// </summary>
    /// <param name="readyForGame"></param>
    [Command]
    public void CmdDoSetReadyForGame(bool readyForGame)
    {
        isReadyForGame = readyForGame;
    }

    /// <summary>
    /// Update the teamTag variable in the server so the hook can update the clients.
    /// </summary>
    /// <param name="teamTag"></param>
    [Command]
    public void CmdDoSetTeam(GameLevelSceneManager.TEAMTAG teamTag)
    {
        this.teamTag = teamTag;
    }

    /// <summary>
    /// Update the ship type variable in the server so the hook can update the clients.
    /// </summary>
    /// <param name="type"></param>
    [Command]
    public void CmdDoSetShipType(ShipType type)
    {
        this.type = type;
    }

    /// <summary>
    /// Internal method to hide or display the pause menu.
    /// </summary>
    private void TogglePauseMenu()
    {
        if (isPause)
        {
            GameLevelEventManager.PauseMenuResumeSelectedEvent -= OnPauseMenuResumeSelected;
            GameLevelEventManager.PauseMenuConfirmQuitSelectedEvent -= sceneManager.QuitGame;
            sceneManager.HideAllMenus();
            Destroy(canvasGameObject);
            canvasGameObject = null;
            Physics.IgnoreLayerCollision(8, 5, false);
        }
        else
        {
            Physics.IgnoreLayerCollision(8, 5, true);
            canvasGameObject = Instantiate(pauseCanvasPrefab);
            GameLevelEventManager.PauseMenuResumeSelectedEvent += OnPauseMenuResumeSelected;
            GameLevelEventManager.PauseMenuConfirmQuitSelectedEvent += sceneManager.QuitGame;
            sceneManager.SetUIManager(canvasGameObject.GetComponent<GameLevelUIManager>());
            sceneManager.DisplayPauseMenu();
        }
        isPause = !isPause;
    }

    /// <summary>
    /// This method will be called when the player selected the "Resume" 
    /// </summary>
    public void OnPauseMenuResumeSelected()
    {
        GameLevelEventManager.PauseMenuResumeSelectedEvent -= OnPauseMenuResumeSelected;
        Assert.IsTrue(isPause);
        Assert.IsNotNull(canvasGameObject);
        TogglePauseMenu();
    }

    // Override the onShooProjectile to provide sounds when shots are fired.
    public override void OnShootProjectile(Vector3 projectileOrigin)
    {
        base.OnShootProjectile(projectileOrigin);
        AudioSource.PlayClipAtPoint(projectileSound, transform.TransformPoint(projectileOrigin));
    }

    // Override onStartReloading to provide reload sounds
    public override void OnStartReloading()
    {
        base.OnStartReloading();
    }

    // Override onDamageRecieved to provide positional audio of the impact
    public override void OnDamageReceived(float damage, Vector3 position)
    {
        base.OnDamageReceived(damage, position);
        AudioSource.PlayClipAtPoint(impactSound, position);
        if (health <= 0)
        {
            transform.position = sceneManager.GetSpawnPosition(teamTag);
        }
    }

    /// <summary>
    /// Recieve this message when the ends by either winning or losing
    /// This is used to show the win/lose dialog
    /// </summary>
    /// <param name="win"></param>
    public override void OnGameEnded(bool win)
    {
        gameEnded = true;
        Physics.IgnoreLayerCollision(8, 5, true);
        canvasGameObject = Instantiate(pauseCanvasPrefab);
        sceneManager.SetUIManager(canvasGameObject.GetComponent<GameLevelUIManager>());
        sceneManager.DisplayGameEndMenu();
    }

    // Override this method to provide extra functionality when the player enters a trigger.
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
