using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour, GameLevelSceneManagerDelegate, GunControllerDelegate, DamageReceiver{

    public Rigidbody rigidBody;
    public GameObject cameraGameObject;
    public GameObject canvasGameObject;

    public GameObject projectilePrefab;

    private GameLevelSceneManager sceneManager;
    private GunController gunController;
    private bool isPause;
    private float health;

	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
        {
            sceneManager = canvasGameObject.GetComponent<GameLevelSceneManager>();
            gunController = GetComponent<GunController>();
            gunController.SetGunControllerDelegate(this);
            sceneManager.SetDelegate(this);
            sceneManager.HideAllMenus();
            isPause = false;
            health = 100f;
        }
        else
        {
            GameObject.Destroy(cameraGameObject);
            GameObject.Destroy(canvasGameObject);
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

        if (isPause)
        {
            return;
        }

        float dStrafe = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_LEFT_X) * 10;
        float dForward = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_LEFT_Y) * 10;
        float dElevate = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_RIGHT_Y) * 10;
        float dRotate = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_RIGHT_X) * 10;

        bool fireTrigger = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.TRIGGER_R2) > 0.5f;
        if (fireTrigger)
        {
            gunController.PressTriger();
        }

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
            sceneManager.DisplayPauseMenu();
        }
        isPause = !isPause;
    }

    public void OnMenuDismissed()
    {
        sceneManager.HideAllMenus();
        isPause = false;
    }

    //Method that will fire a bullet.
    private void DoFire()
    {
        Vector3 bulletOrigin = transform.position + transform.forward * 5;
        Quaternion bulletOrientation = transform.rotation;
        GameObject bullet = (GameObject)Instantiate(projectilePrefab, bulletOrigin, bulletOrientation);
        bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroy(bullet, 1.5f);
    }

    // Make the server call the RPC so that it can send the message
    // to all the remote instances.
    [Command]
    void CmdDoFire()
    {
        RpcDoFire();
    }

    // Do not fire if this comes from the server. All the shots for the local client
    // Should be trigered by directly calling DoFie.
    [ClientRpc]
    void RpcDoFire()
    {
        if (isLocalPlayer)
        {
            return;
        }
        DoFire();
    }

    public void ShootBullet()
    {
        DoFire();
        CmdDoFire();
    }

    public void StartReloading()
    {
    }

    public void RecievedDamage(float damage)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            transform.position = Vector3.zero;
            health = 100;
        }
    }
}
