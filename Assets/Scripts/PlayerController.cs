using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : ShipController, GameLevelSceneManagerDelegate {

    public GameObject cameraGameObject;
    public GameObject pauseCanvasPrefab;
    public GameObject shipConfigCanvasPrefab;

    private new Collider collider;
    private GameLevelSceneManager sceneManager;
    private GameObject canvasGameObject;
    private bool isPause;

    // Use this for initialization
    void Start()
    {
        Init();
        if (isLocalPlayer)
        {
            isPause = false;
        }
        else
        {
            GameObject.Destroy(cameraGameObject);
        }
        collider = GetComponent<SphereCollider>();

        DisplayShipConfigMenu();
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

    private void DisplayShipConfigMenu()
    {
        collider.enabled = false;
        gameRenderer.enabled = false;
        canvasGameObject = Instantiate(shipConfigCanvasPrefab);
        sceneManager = canvasGameObject.GetComponent<GameLevelSceneManager>();
        sceneManager.SetDelegate(this);
        sceneManager.DisplayShipSelectMenu();
    }

    private void DismissShipConfigMenu()
    {
        sceneManager.HideAllMenus();
        Destroy(canvasGameObject);
        canvasGameObject = null;
        collider.enabled = true;
        gameRenderer.enabled = true;
    }

    public void OnShipConfigMenuDismissed()
    {
        DismissShipConfigMenu();
    }

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
            sceneManager = canvasGameObject.GetComponent<GameLevelSceneManager>();
            sceneManager.SetDelegate(this);
            sceneManager.DisplayPauseMenu();
        }
        isPause = !isPause;
    }

    public void OnPauseMenuDismissed()
    {
        sceneManager.HideAllMenus();
        isPause = false;
    }

    public void OnTeamSelectMenuDismissed(LevelSceneManager.TEAMTAG teamTag)
    {
        setTeam(teamTag);
    }
}
