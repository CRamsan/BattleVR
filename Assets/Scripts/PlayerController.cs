using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour {

    public Rigidbody rigidBody;
    public GameObject cameraGameObject;
    public GameObject canvasGameObject;

    private GameLevelSceneManager sceneManager;
    private bool isPause;

	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
        {
            sceneManager = canvasGameObject.GetComponent<GameLevelSceneManager>();
            sceneManager.HideAllMenus();
            isPause = false;
        }
        else
        {
            GameObject.Destroy(cameraGameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        if (!isLocalPlayer)
        {
            return;
        }

        bool pausePressed = InputManager.WasPressed(InputManager.CONTROLLER_BUTTON.START);

        if (pausePressed)
        {
            TogglePauseMenu();
            return;
        }

        float dStrafe = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_LEFT_X);
        float dForward = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_LEFT_Y);
        float dElevate = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_RIGHT_Y);
        float dRotate = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_RIGHT_X) * 5;

        Vector2 leftStickVector = new Vector2(dForward, dStrafe);
        Vector2 rightStickVector = new Vector2(dElevate, dRotate);

        if (rightStickVector.magnitude >= 0.2)
        {
            Quaternion newRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, dRotate, 0));
            rigidBody.MoveRotation(newRotation);
        }
        else
        {
            dElevate = 0;
        }

        if (leftStickVector.magnitude < 0.2)
        {
            dStrafe = 0;
            dForward = 0;
        }
        if (dStrafe != 0 || dForward != 0 || dElevate != 0)
        {
            Vector3 dVector = new Vector3(dStrafe, dElevate, dForward) / 10;
            rigidBody.MovePosition(transform.position + transform.TransformDirection(dVector));
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
            sceneManager.DisplayConfirmation();
        }
        isPause = !isPause;
    }
}
