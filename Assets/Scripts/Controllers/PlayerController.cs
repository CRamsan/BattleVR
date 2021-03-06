﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This controller will provide the input management for movement and UI for an agent controlled by a human player.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class PlayerController : ShipController {

    public GameObject hudGameObject;
    public GameObject enemyMarkerPrefab;
    public GameObject friendlyMarkerPrefab;

    public GameObject pauseCanvasPrefab;
    public GameObject shipConfigCanvasPrefab;
    public AudioClip impactSound;
    public AudioClip projectileSound;
    public Vector3 canvasMenuPosition;
    public Vector3 canvasMenuScale;
    public GameObject headGameObject;
    public Vector3 cameraAnchor;

    private GameLevelSceneManager sceneManager;
    private GameObject canvasGameObject;
    private SphereCollider sphereCollider;

    private bool isPause;
    private bool isTeamSelected;
    private Dictionary<ShipController, GameObject> enemyShipMap;
    private Dictionary<ShipController, GameObject> allyShipMap;
    private Dictionary<GameObject, GameObject> objectiveMap;

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
            sphereCollider = GetComponent<SphereCollider>();
            enemyShipMap = new Dictionary<ShipController, GameObject>();
            allyShipMap = new Dictionary<ShipController, GameObject>();
            GameLevelEventManager.ShipDestroyedEvent += HandleShipExitVecinity;
        }
        else
        {
            GetComponent<SphereCollider>().enabled = false;

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

            Vector3 cameraLocalAnchor = transform.TransformPoint(cameraAnchor);
            headGameObject.transform.position = cameraLocalAnchor + transform.TransformVector(new Vector3(leftStickVector.x/-25, 0, leftStickVector.z/-25));
        }

        HandleInput(leftStickVector.magnitude >= 0.2 ? leftStickVector : Vector3.zero,
                    rotationStickVector.magnitude >= 0.2 ? rotationStickVector : Vector3.zero);
    }

    void FixedUpdate()
    {
    }

    /// <summary>
    /// Method called when this script becomes active
    /// </summary>
    void OnEnable()
    {

    }

    /// <summary>
    /// Method that gets called when this script gets disabled
    /// </summary>
    void OnDisable()
    {
        if (isLocalPlayer)
        {
            GameLevelEventManager.ShipDestroyedEvent -= HandleShipExitVecinity;
        }
    }

    /// <summary>
    /// Logic that needs to be called after the update method. Here I am adding the 
    /// logic for setting the marker positions.
    /// </summary>
    void LateUpdate()
    {
        float markerCount = enemyShipMap.Count;
        foreach (KeyValuePair<ShipController, GameObject> entry in enemyShipMap)
        {
            Vector3 enemyDirection = (entry.Key.transform.position - Camera.main.transform.position).normalized;
            entry.Value.transform.position = Camera.main.transform.position + (enemyDirection * 10f);
            entry.Value.transform.LookAt(Camera.main.transform);
            float dist = 1f / ((Vector3.Distance(transform.position, entry.Value.transform.position) / 100f) + 1f);
            entry.Value.transform.localScale = new Vector3(dist, dist, dist);
        }
        foreach (KeyValuePair<ShipController, GameObject> entry in allyShipMap)
        {
            Vector3 allyDirection = (entry.Key.transform.position - Camera.main.transform.position).normalized;
            entry.Value.transform.position = Camera.main.transform.position + (allyDirection * 10f);
            entry.Value.transform.LookAt(Camera.main.transform);
            float dist = 1f / ((Vector3.Distance(transform.position, entry.Value.transform.position) / 100f) + 1f);
            entry.Value.transform.localScale = new Vector3(dist, dist, dist);
        }
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
        }
        else
        {
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
        canvasGameObject = Instantiate(pauseCanvasPrefab);
        sceneManager.SetUIManager(canvasGameObject.GetComponent<GameLevelUIManager>());
        sceneManager.DisplayGameEndMenu();
    }

    // Override this method to provide extra functionality when the player enters a trigger.
    public override void OnTriggerEnter(Collider other)
    {
        ShipController controller = other.transform.parent.GetComponent<ShipController>();
        // When a shipController enters the radar collider
        if (controller != null)
        {
            Assert.IsTrue(isLocalPlayer);
            Assert.IsFalse(Network.isServer);
            HandleShipEnterVecinity(controller);
        }
        else 
        {
            base.OnTriggerEnter(other);
        }
    }

    // Override this method to provide extra functionality when the player enters a trigger.
    public void OnTriggerExit(Collider other)
    {
        ShipController controller = other.transform.parent.GetComponent<ShipController>();
        // When a shipController enters the radar collider
        if (controller != null)
        {
            Assert.IsTrue(isLocalPlayer);
            Assert.IsFalse(Network.isServer);
            HandleShipExitVecinity(controller);
        }
        else
        {
        }
    }

    // This method will check if the enemy ship is already a know one. 
    // If it is not we will create a marker for it and add it to the map.
    private void HandleShipEnterVecinity(ShipController ship)
    {
        if (ship.GetTeam() != GetTeam() && !enemyShipMap.ContainsKey(ship))
        {
            GameObject EnemyMarker = (GameObject)Instantiate(enemyMarkerPrefab, hudGameObject.transform);
            EnemyMarker.transform.localScale = new Vector3(1, 1, 1);
            enemyShipMap.Add(ship, EnemyMarker);
        }
    }

    // When a ship leaves the vecinity then remove the marker and the ship from the map.
    private void HandleShipExitVecinity(ShipController ship)
    {
        if (ship.GetTeam() != GetTeam())
        {
            Dictionary<ShipController, GameObject> shipMap;
            shipMap = enemyShipMap;
            if (shipMap.ContainsKey(ship))
            {
                GameObject shipMarker = shipMap[ship];
                shipMap.Remove(ship);
                Destroy(shipMarker);
            }
        }
    }

    /// <summary>
    /// This method is called when a new ship is created for this team
    /// </summary>
    /// <param name="ship"></param>
    public override void OnFriendlyShipSpawned(ShipController ship)
    {
        GameObject FriendlyMarker = (GameObject)Instantiate(friendlyMarkerPrefab, hudGameObject.transform);
        FriendlyMarker.transform.localScale = new Vector3(1, 1, 1);
        allyShipMap.Add(ship, FriendlyMarker);
    }

    /// <summary>
    /// This method is called when a friendly ship is destroyed
    /// </summary>
    /// <param name="ship"></param>
    public override void OnFriendlyShipDestroyed(ShipController ship)
    {
        Assert.IsTrue(allyShipMap.ContainsKey(ship));
        GameObject shipMarker = allyShipMap[ship];
        allyShipMap.Remove(ship);
        Destroy(shipMarker);
    }
}
