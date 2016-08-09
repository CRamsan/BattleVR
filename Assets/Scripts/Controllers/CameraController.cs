using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// This controller provides functionality for the camera in both VR and non-VR enviroments. 
/// </summary>
public class CameraController : MonoBehaviour {

    public float reticleMaxDistance = 20f;
    public GameObject reticlePrefab;

    private GameObject reticleOject;
    private RaycastHit hit; 

	// Use this for initialization
	void Start () {
        reticleOject = Instantiate(reticlePrefab);
        reticleOject.transform.SetParent(transform);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 originPosition;

        if (VRSettings.enabled)
        {
            originPosition = InputTracking.GetLocalPosition(VRNode.CenterEye);
        }
        else
        {
            originPosition = transform.localPosition;
        }


        if (Physics.Raycast(Vector3.zero, Vector3.forward, out hit, reticleMaxDistance))
        {
            reticleOject.transform.localPosition = (Vector3.forward * hit.distance);
        }
        else
        {
            reticleOject.transform.localPosition = (Vector3.forward * reticleMaxDistance);
        }
    }
}
