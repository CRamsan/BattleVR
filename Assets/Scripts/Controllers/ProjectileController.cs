using UnityEngine;

/// <summary>
/// This controller will provide the logic for handling a projectile. 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{

    public float speed;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        DamageReceiver cont = col.gameObject.GetComponent<DamageReceiver>();
        if (cont != null)
        {
            cont.onDamageReceived(10f, transform.position);
        }
        Destroy(gameObject);
    }
}
