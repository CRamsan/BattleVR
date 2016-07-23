using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : ShipController, GameLevelSceneManagerDelegate {

    public GameObject cameraGameObject;
    public GameObject canvasGameObject;

    private GameLevelSceneManager sceneManager;
    private bool isPause;

    // Use this for initialization
    void Start () {
        Init();
        if (isLocalPlayer)
        {
            sceneManager = canvasGameObject.GetComponent<GameLevelSceneManager>();
            sceneManager.SetDelegate(this);
            sceneManager.HideAllMenus();
            isPause = false;
        }
        else
        {
            GameObject.Destroy(cameraGameObject);
            GameObject.Destroy(canvasGameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }

        bool pausePressed = InputManager.WasActionPressed(InputManager.CONTROLLER_ACTION.PAUSE);

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

        if (rotationStickVector.magnitude >= 0.2)
        {
            rigidBody.AddRelativeTorque(rotationStickVector * Time.deltaTime * 15);
        }

        if (leftStickVector.magnitude >= 0.2)
        {
            rigidBody.AddRelativeForce(leftStickVector * Time.deltaTime * 2000);
        }
    }

    private void TogglePauseMenu()
    {
        if (isPause)
        {
            sceneManager.HideAllMenus();
        }
        else
        {
            sceneManager.DisplayPauseMenu();
        }
        isPause = !isPause;
    }

    public void OnMenuDismissed()
    {
        sceneManager.HideAllMenus();
        isPause = false;
    }
}
