using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float reticleMaxDistance = 10f;
    public GameObject reticlePrefab;

    private GameObject reticleOject;
    private RaycastHit hit; 

	// Use this for initialization
	void Start () {
        reticleOject = Instantiate(reticlePrefab);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPosition = transform.position;
        Vector3 targerDirection = transform.rotation * (Vector3.forward);

        if (Physics.Raycast(targetPosition, targerDirection, out hit, reticleMaxDistance))
        {
            reticleOject.transform.position = targetPosition + (targerDirection * hit.distance);
        }
        else
        {
            reticleOject.transform.position = targetPosition + (targerDirection * reticleMaxDistance);
        }
    }
}
