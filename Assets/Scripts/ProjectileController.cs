using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

    public float speed;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * Time.deltaTime * speed, ForceMode.Impulse);
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        DamageReceiver cont = col.gameObject.GetComponent<DamageReceiver>();
        if (cont != null)
        {
            cont.RecievedDamage(10f);
        }
        Destroy(gameObject);
    }
}
