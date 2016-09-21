using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.VR;

/// <summary>
/// This controller is the base controller for any ship, either AI or human controlled. It is independant from the
/// input, but it will handle the phisycs and weapon controll.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(LODGroup))]
public abstract class ShipController : NetworkBehaviour, GunControllerDelegate, DamageReceiver
{

    public enum ShipType
    {
        NONE,
        FIGHTER,
        FRIGATE,
        DESTROYER
    }

    // This should be moved out of here and into the GunController
    public GameObject projectilePrefab;

    public float roll = 10;
    public float pitch = 10;
    public float yawn = 10;
    public float strafe = 10;
    public float thrust = 10;
    public float acceleration = 10;

    protected GunController gunController;
    protected GameObject rendererGameObject;
    protected GameObject gameLODGroup;
    protected Rigidbody gameRigidBody;
    protected TeamController teamController;
    protected float health;
    protected bool isAI;
    protected bool gameEnded;
    protected Renderer[] rendererList;

    [SyncVar(hook = "SetTeam")]
    protected GameLevelSceneManager.TEAMTAG teamTag;

    [SyncVar(hook = "SetShipType")]
    protected ShipType type;

    [SyncVar(hook = "onReadyForGameChanged")]
    protected bool isReadyForGame;

    private Color tempColor;
    private Vector3 throttle;
    private bool hasInit;

    // This method should only be called once and it should be called before any other method. 
    // As a precausion this method will only assert if called multiple times.
    private void Init()
    {
        Assert.IsFalse(hasInit);
        if (isLocalPlayer)
        {
        }
        else
        {
        }

        gameRigidBody = GetComponent<Rigidbody>();
        gunController = GetComponent<GunController>();
        gunController.gunControllerDelegate = this;

        health = 100f;
        rendererList = new Renderer[0];
        hasInit = true;
        gameEnded = false;
    }

    protected void SafeInit()
    {
        if (!hasInit)
            Init();
    }

    // Set the team for this ship and apply any logic needed
    public void SetTeam(GameLevelSceneManager.TEAMTAG teamTag)
    {
        this.teamTag = teamTag;
    }

    /// <summary>
    /// Set the team controller so this ship can communicate
    /// with the rest of the team
    /// </summary>
    /// <param name="controller"></param>
    public void SetTeamController(TeamController controller)
    {
        teamController = controller;
    }

    /// <summary>
    /// Call the team variable and update the gameobjects accordingly.
    /// This has to be separate setTeam() because the team variable can be 
    /// synced over the network. 
    /// </summary>
    public void RefreshTeamState()
    {
        if (!hasInit)
            Init();
        Material teamMaterial;
        switch (this.teamTag)
        {
            case GameLevelSceneManager.TEAMTAG.BLUE:
                teamMaterial = (Material)AssetManager.instance.GetAsset(AssetManager.ASSET.TEAM_BLUE_MATERIAL);
                break;
            case GameLevelSceneManager.TEAMTAG.RED:
                teamMaterial = (Material)AssetManager.instance.GetAsset(AssetManager.ASSET.TEAM_RED_MATERIAL);
                break;
            default:
                throw new UnityException();
        }
        Assert.IsTrue(rendererList.Length > 0);
        foreach (Renderer rend in rendererList)
        {
            rend.material = teamMaterial;
        }
    }

    // Get the team value
    public GameLevelSceneManager.TEAMTAG GetTeam()
    {
        return this.teamTag;
    }

    // Set the ShipType value
    public void SetShipType(ShipType type)
    {
        this.type = type;
    }

    /// <summary>
    /// Call this method tp update the gameObject to behave as
    /// the correct ship type.
    /// </summary>
    public void RefreshShipType()
    {
        if (!hasInit)
            Init();
        if (gameLODGroup != null)
        {
            gameLODGroup.SetActive(false);
            Destroy(gameLODGroup);
            gameLODGroup = null;
        }

        switch (this.type)
        {
            case ShipType.FIGHTER:
                gameLODGroup = (GameObject)Instantiate(AssetManager.instance.GetAsset(AssetManager.ASSET.FIGHTER_LODGROUP),
                    transform.position, 
                    transform.rotation);
                break;
            case ShipType.FRIGATE:
                gameLODGroup = (GameObject)Instantiate(AssetManager.instance.GetAsset(AssetManager.ASSET.FRIGATE_LODGROUP),
                    transform.position,
                    transform.rotation);
                break;
            case ShipType.DESTROYER:
                gameLODGroup = (GameObject)Instantiate(AssetManager.instance.GetAsset(AssetManager.ASSET.FRIGATE_LODGROUP),
                    transform.position,
                    transform.rotation);
                break;
            default:
                throw new UnityException();
        }
        gameLODGroup.transform.SetParent(transform);
        rendererList = (Renderer[])gameLODGroup.GetComponentsInChildren<Renderer>();
    }

    // Get the ShipType value
    public ShipType GetShipType()
    {
        return this.type;
    }

    public bool IsReadyForGame()
    {
        return isReadyForGame;
    }

    public void SetReadyForGame(bool readyForGame)
    {
        isReadyForGame = readyForGame;
    }

    // This method will be called when the isReadyForGame variable changes.
    public void onReadyForGameChanged(bool readyForGame)
    {
        isReadyForGame = readyForGame;
        if (isReadyForGame)
        {
            RefreshShipType();
            RefreshTeamState();
        }
    }

    /// <summary>
    /// This method will transform the input vectors into forces. All vectors should have a magitude MAX of 1.0f.
    /// </summary>
    /// <param name="dTranslation"></param>
    /// <param name="dRotation"></param>
    protected void HandleInput(Vector3 dTranslation, Vector3 dRotation)
    {
#if UNITY_EDITOR
        Assert.IsTrue(dTranslation.y == 0); //Currently there is no support for ascending and descending
        // Verify all other vectors are less or equal to 1f
        Assert.IsTrue(Mathf.Abs(dTranslation.x) <= 1);
        Assert.IsTrue(Mathf.Abs(dTranslation.z) <= 1);
        Assert.IsTrue(Mathf.Abs(dRotation.x) <= 1);
        Assert.IsTrue(Mathf.Abs(dRotation.y) <= 1);
        Assert.IsTrue(Mathf.Abs(dRotation.z) <= 1);
#endif
        // TODO Refactor this code to make it more organized and optimized.
        // This code was modified to quickly test the variables for 
        // yawn, pitch, roll, etc.
        if (type == ShipType.FIGHTER)
        {
            float throttleZ = throttle.z + (acceleration * dTranslation.z / 100);

            throttle.z = throttleZ > 0 ? Mathf.Min(throttleZ, 1f) : 0;
            gameRigidBody.AddRelativeForce(throttle * thrust * Time.deltaTime, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 modifiedForce = new Vector3(dTranslation.x * strafe, 0, dTranslation.z * thrust);
            gameRigidBody.AddRelativeForce(modifiedForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        Vector3 modifiedRotation = new Vector3(dRotation.x * pitch, dRotation.y * yawn, dRotation.z * roll);
        gameRigidBody.AddRelativeTorque(modifiedRotation * Time.deltaTime, ForceMode.VelocityChange);
    }

    //Method that will fire a bullet.
    private void DoFire(Vector3 projectileOrigin)
    {
        Quaternion bulletOrientation;
        if (ShipType.DESTROYER == type && isLocalPlayer)
        {
            if (VRSettings.enabled)
            {
                bulletOrientation = InputTracking.GetLocalRotation(VRNode.CenterEye);
            }
            else
            {
                bulletOrientation = Camera.main.transform.rotation;
            }
        }
        else
        {
            bulletOrientation = transform.rotation;
        }
        Vector3 bulletOrigin = transform.TransformPoint(projectileOrigin);
        GameObject bullet = (GameObject)Instantiate(projectilePrefab, bulletOrigin, bulletOrientation);
        if (isLocalPlayer)
        {
            bullet.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Extrapolate;
        }
        Destroy(bullet, 1.5f);
    }

    // Make the server call the RPC so that it can send the message
    // to all the remote instances.
    [Command]
    void CmdDoFire(Vector3 projectileOrigin)
    {
        RpcDoFire(projectileOrigin);
    }

    // Do not fire if this comes from the server. All the shots for the local client
    // Should be trigered by directly calling DoFie.
    [ClientRpc]
    void RpcDoFire(Vector3 projectileOrigin)
    {
        if (isLocalPlayer || isAI)
        {
            return;
        }
        DoFire(projectileOrigin);
    }

    //Shoot a projectile locally and send a message to the server to trigger a shot on the other clients.
    public virtual void onShootProjectile(Vector3 projectileOrigin)
    {
        DoFire(projectileOrigin);
        CmdDoFire(projectileOrigin);
    }

    // This method will be called when the gun needs to start reloading.
    public virtual void onStartReloading()
    {
    }

    //This nethod will be called when this ship takes some damage
    public virtual void onDamageReceived(float damage, Vector3 position)
    {
        health -= damage;
        if (health <= 0)
        {
            teamController.OnUnitDestroyed(gameObject);
        }
    }

    // This method will be called when the ship colliders with other interactable objects such as weapons or items.
    public virtual void OnTriggerEnter(Collider other)
    {
        WeaponController controller = other.gameObject.GetComponent<WeaponController>();
        if (controller != null && gunController != null)
        {
            gunController.SetWeapon(controller);
            Destroy(controller.gameObject);
        }
    }

    /// <summary>
    /// Recieve this message when the ends by either winning or losing
    /// </summary>
    /// <param name="win"></param>
    public virtual void OnGameEnded(bool win)
    {
    }
}
