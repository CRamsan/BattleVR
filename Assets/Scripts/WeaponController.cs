using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

    public float rechamberTime;
    public float reloadTime;
    public int magSize;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void WeaponDestroy()
    {
        Destroy(gameObject);
    }
}
