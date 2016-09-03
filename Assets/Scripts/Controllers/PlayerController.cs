using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// This controller will provide the input management for movement and UI for an agent controlled by a human player.
/// </summary>
public class PlayerController : ShipController, GameLevelSceneManagerDelegate {

    public GameObject pauseCanvasPrefab;
    public GameObject shipConfigCanvasPrefab;
    public AudioClip impactSound;
    public AudioClip projectileSound;

    private Collider playerCollider;
    private GameLevelSceneManager sceneManager;
    private GameObject canvasGameObject;
    private bool isPause;
    private bool isTeamSelected;

    [SyncVar(hook = "onReadyForGameChanged")]
    protected bool isReadyForGame;

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
            playerCollider = GetComponent<SphereCollider>();
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
        CmdDoSetTeam(teamTag);
        isTeamSelected = true;
        sceneManager.DisplayShipSelectMenu();
    }

    /// <summary>
    /// This method will be called when the user selected a ship from the UI. This method will set the variables 
    /// and spawn the player in the correct position.
    /// </summary>
    public void OnShipConfigMenuShipSelected(ShipController.ShipType type)
    {
        CmdDoSetShipType(type);
        DismissShipConfigMenu();
        transform.position = sceneManager.GetSpawnPosition(teamTag);
        CmdDoSetReadyForGame(true);
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

    // Overrride for SetTeam. This allows this controller to change the team variable 
    // without trigering a refresh of the whole game object.
    public override void SetTeam(GameLevelSceneManager.TEAMTAG teamTag)
    {
        this.teamTag = teamTag;
    }
    
    // Override for SetShipType that allows this controller to change the ship type
    // variable without trigering a refresh of the whole game object.
    public override void SetShipType(ShipType type)
    {
        this.type = type;
    }

    // This method will be called when the isReadyForGame variable changes.
    public void onReadyForGameChanged(bool readyForGame)
    {
        isReadyForGame = readyForGame;
        if (isReadyForGame)
        {
            RefreshTeamState();
            RefreshShipType();
        }
    }

    // Override the onShooProjectile to provide sounds when shots are fired.
    public override void onShootProjectile()
    {
        base.onShootProjectile();
        AudioSource.PlayClipAtPoint(projectileSound, transform.forward + transform.position);
    }

    // Override onStartReloading to provide reload sounds
    public override void onStartReloading()
    {
        base.onStartReloading();
    }

    // Override onDamageRecieved to provide positional audio of the impact
    public override void onDamageReceived(float damage, Vector3 position)
    {
        base.onDamageReceived(damage, position);
        AudioSource.PlayClipAtPoint(impactSound, position);
    }

    // Override this method to provide extra functionality when the player enters a trigger.
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
