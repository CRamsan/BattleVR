using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour, GameLevelSceneManagerDelegate, GunControllerDelegate, DamageReceiver{

    public Rigidbody rigidBody;
    public GameObject cameraGameObject;
    public GameObject canvasGameObject;
    public GameObject rendererGameObject;

    public GameObject projectilePrefab;
    public Vector3[] bulletSpawn;

    private GameLevelSceneManager sceneManager;
    private GunController gunController;
    private bool isPause;
    private float health;
    private int bulletSpawnIndex;

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

    //Method that will fire a bullet.
    private void DoFire()
    {
        Vector3 bulletOrigin = transform.TransformPoint(bulletSpawn[bulletSpawnIndex]);
        bulletSpawnIndex++;
        if (bulletSpawnIndex >= bulletSpawn.Length)
        {
            bulletSpawnIndex = 0;
        }
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

    void OnTriggerEnter(Collider other)
    {
        WeaponController controller = other.gameObject.GetComponent<WeaponController>();
        if (controller != null)
        { }
    }
}
