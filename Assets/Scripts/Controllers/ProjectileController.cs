using UnityEngine;

/// <summary>
/// This controller will provide the logic for handling a projectile. 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    public GameObject collisionPrefab;
    public bool clientMode;
    public float speed;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // TODO Lets use this to make the projectiles that are not from the current player
        // less computational intensive. We only need to be precise with our own projectiles.
        if (clientMode)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }

        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
    }

    void Update()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        // We only want to listen for hits from the current client. 
        // All other damage is done by synchronizing the health
        if (clientMode)
        {
            DamageReceiver cont = col.gameObject.GetComponent<DamageReceiver>();
            if (cont != null)
            {
                cont.OnDamageReceived(10f, transform.position);
                GameLevelEventManager.OnPlayerProjectileHit();
            }
        }
        Destroy(gameObject);
        Destroy(Instantiate(collisionPrefab, col.contacts[0].point, transform.rotation), 5f);
    }
}
