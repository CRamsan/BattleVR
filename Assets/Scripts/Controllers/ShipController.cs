using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// This controller is the base controller for any ship, either AI or human controlled. It is independant from the
/// input, but it will handle the phisycs and weapon controll.
/// </summary>
public class ShipController : NetworkBehaviour, GunControllerDelegate, DamageReceiver
{

    public enum ShipType
    {
        FIGHTER,
        FRIGATE,
        BATTLECRUISER
    }

    // This should be moved out of here and into the GunController
    public GameObject projectilePrefab;

    protected GameLevelSceneManager.TEAMTAG teamTag;
    protected GunController gunController;
    protected GameObject rendererGameObject;
    protected Renderer gameRenderer;
    protected MeshFilter gameMeshFilter;
    protected Rigidbody gameRigidBody;
    protected float health;
    protected bool isAI;
    protected ShipType type;

    private Color tempColor;
    private Vector3 throttle;

    // This method should only be called once and it should be called before any other method.
    protected void Init()
    {
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
        gameRenderer = GetComponentInChildren<Renderer>();
        gameMeshFilter = GetComponentInChildren<MeshFilter>();
        type = ShipType.FRIGATE;
    }

    // Set the team for this ship and apply any logic needed
    public void setTeam(GameLevelSceneManager.TEAMTAG teamTag)
    {
        this.teamTag = teamTag;
        switch (this.teamTag)
        {
            case GameLevelSceneManager.TEAMTAG.BLUE:
                gameRenderer.material = (Material)AssetManager.instance.GetAsset(AssetManager.ASSET.TEAM_BLUE_MATERIAL);
                break;
            case GameLevelSceneManager.TEAMTAG.RED:
                gameRenderer.material = (Material)AssetManager.instance.GetAsset(AssetManager.ASSET.TEAM_RED_MATERIAL);
                break;
            default:
                throw new UnityException();
        }
    }

    // 
    public void setShipType(ShipType type)
    {
        this.type = type;
        switch (this.type)
        {
            case ShipType.FIGHTER:
                gameMeshFilter.mesh = (Mesh)AssetManager.instance.GetAsset(AssetManager.ASSET.FIGHTER_MODEL);
                break;
            case ShipType.FRIGATE:
                gameMeshFilter.mesh = (Mesh)AssetManager.instance.GetAsset(AssetManager.ASSET.FRIGATE_MODEL);
                break;
            default:
                throw new UnityException();
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
        if (dTranslation.x > 1.0f || dTranslation.y > 1.0f || dTranslation.z > 1.0f)
        {
            Assert.IsTrue(false);
            throw new AssertionException("Invalid vector", "dTranslation has one of it't components greater than 1");
        }
        if (dRotation.x > 1.0f || dRotation.y > 1.0f || dRotation.z > 1.0f)
        {
            Assert.IsTrue(false);
            throw new AssertionException("Invalid vector", "dRotation has one of it't components greater than 1");
        }
#endif
        if (type == ShipType.FIGHTER)
        {
            float throttleZ = throttle.z + dTranslation.z;

            throttle.z = throttleZ > 0 ? Mathf.Min(throttleZ, 1f) : 0;
            gameRigidBody.AddRelativeForce(throttle * Time.deltaTime * 4000);
        }
        else
        {
            gameRigidBody.AddRelativeForce(dTranslation * Time.deltaTime * 2000);
        }
        gameRigidBody.AddRelativeTorque(dRotation * Time.deltaTime * 15);
    }

    //Method that will fire a bullet.
    private void DoFire()
    {
        Quaternion bulletOrientation = transform.rotation;
        Vector3 bulletOrigin = transform.TransformPoint(Vector3.forward * 2);
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
        if (isLocalPlayer || isAI)
        {
            return;
        }
        DoFire();
    }

    //Shoot a projectile locally and send a message to the server to trigger a shot on the other clients.
    public void onShootProjectile()
    {
        DoFire();
        CmdDoFire();
    }

    // This method will be called when the gun needs to start reloading.
    public void onStartReloading()
    {
    }

    //This nethod will be called when this ship takes some damage
    public void onDamageReceived(float damage)
    {
        StartCoroutine(TakeDamageEnumerator());

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
        if (controller != null && gunController != null)
        {
            gunController.SetWeapon(controller);
            Destroy(controller.gameObject);
        }
    }

    //Corouitine to flash the renderer when the ship takes damage.
    IEnumerator TakeDamageEnumerator()
    {
        tempColor = gameRenderer.material.color;
        gameRenderer.material.color = Color.blue;
        yield return new WaitForSeconds(.1f);
        gameRenderer.material.color = tempColor;
    }
}
