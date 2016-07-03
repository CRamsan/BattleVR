using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

    public float speed;

    void Start()
    {
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
