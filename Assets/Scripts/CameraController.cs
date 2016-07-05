using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float reticleMaxDistance = 20f;
    public GameObject reticlePrefab;

    private GameObject reticleOject;
    private RaycastHit hit; 

	// Use this for initialization
	void Start () {
        reticleOject = Instantiate(reticlePrefab);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 originPosition, originDirection;

        if (VRSettings.enabled)
        {
            originPosition = transform.position + InputTracking.GetLocalPosition(VRNode.CenterEye);
            originDirection = transform.TransformDirection(InputTracking.GetLocalRotation(VRNode.CenterEye) * (Vector3.forward));
        }
        else
        {
            originPosition = transform.position;
            originDirection = transform.rotation * (Vector3.forward);
        }


        if (Physics.Raycast(originPosition, originDirection, out hit, reticleMaxDistance))
        {
            reticleOject.transform.localPosition = originPosition + (originDirection * hit.distance);
        }
        else
        {
            reticleOject.transform.localPosition = originPosition + (originDirection * reticleMaxDistance);
        }
    }
}
