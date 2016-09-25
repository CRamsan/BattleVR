using UnityEngine;

/// <summary>
/// This controller will provide the logic for handling a projectile. 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    public GameObject collisionPrefab;
    public float speed;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
    }

    void Update()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        DamageReceiver cont = col.gameObject.GetComponent<DamageReceiver>();
        if (cont != null)
        {
            cont.OnDamageReceived(10f, transform.position);
        }
        Destroy(gameObject);
        Destroy(Instantiate(collisionPrefab, col.contacts[0].point, transform.rotation), 5f);
    }
}
