using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ShipController : NetworkBehaviour, GunControllerDelegate, DamageReceiver{

    public enum ShipType {
        FIGHTER,
        FRIGATE,
        BATTLECRUISER
    }

    public Rigidbody rigidBody;
    public GameObject rendererGameObject;

    public GameObject projectilePrefab;
    public Vector3[] bulletSpawn;

    protected GunController gunController;
    protected float health;
    protected int bulletSpawnIndex;
    protected Renderer gameRenderer;

    protected Color tempColor;
    protected bool isAI;
    public ShipType type;

    private Vector3 throttle;

    protected void Init()
    {
        if (isLocalPlayer)
        {
        }
        else
        {
        }

        gunController = GetComponent<GunController>();
        gunController.SetGunControllerDelegate(this);

        health = 100f;
        gameRenderer = GetComponentInChildren<Renderer>();
        type = ShipType.FIGHTER;
    }

    protected void HandleInput(Vector3 dTranslation, Vector3 dRotation)
    {
        if (type == ShipType.FIGHTER)
        {
            float throttleZ = throttle.z + dTranslation.z;

            throttle.z = throttleZ > 0 ? Mathf.Min(throttleZ, 1f) : 0;
            rigidBody.AddRelativeForce(throttle * Time.deltaTime * 2000);
        }
        else
        {
            rigidBody.AddRelativeForce(dTranslation * Time.deltaTime * 2000);
        }
        rigidBody.AddRelativeTorque(dRotation * Time.deltaTime * 15);
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
        if (isLocalPlayer || isAI)
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

    IEnumerator TakeDamageEnumerator()
    {
        tempColor = gameRenderer.material.color;
        gameRenderer.material.color = Color.blue;
        yield return new WaitForSeconds(.1f);
        gameRenderer.material.color = tempColor;
    }
}
