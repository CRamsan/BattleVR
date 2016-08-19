﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// This controller is the base controller for any ship, either AI or human controlled. It is independant from the
/// input, but it will handle the phisycs and weapon controll.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TrailRenderer))]
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
    protected TrailRenderer trailRenderer;
    protected float health;
    protected bool isAI;
    protected ShipType type;

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
        trailRenderer = GetComponent<TrailRenderer>();

        health = 100f;
        gameRenderer = GetComponentInChildren<Renderer>();
        gameMeshFilter = GetComponentInChildren<MeshFilter>();
        type = ShipType.FRIGATE;
        hasInit = true;
    }

    protected void SafeInit()
    {
        if (!hasInit)
            Init();
    }

    // Set the team for this ship and apply any logic needed
    public void setTeam(GameLevelSceneManager.TEAMTAG teamTag)
    {
        if (!hasInit)
            Init();

        this.teamTag = teamTag;
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
        trailRenderer.material = teamMaterial;
        gameRenderer.material = teamMaterial;
    }

    // Get the team value
    public GameLevelSceneManager.TEAMTAG GetTeam()
    {
        return this.teamTag;
    }

    // Set the ShipType value
    public void setShipType(ShipType type)
    {
        if (!hasInit)
            Init();

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

    // Get the ShipType value
    public ShipType getShipType()
    {
        return this.type;
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
        if (type == ShipType.FIGHTER)
        {
            float throttleZ = throttle.z + (dTranslation.z / 1000);

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
