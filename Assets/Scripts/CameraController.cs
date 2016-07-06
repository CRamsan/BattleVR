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
        reticleOject.transform.SetParent(transform);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 originPosition, originDirection;

        /*if (VRSettings.enabled)
        {
            originPosition = transform.TransformPoint(InputTracking.GetLocalPosition(VRNode.CenterEye));
            originDirection = Quaternion.Euler(InputTracking.GetLocalRotation(VRNode.CenterEye).eulerAngles) * Vector3.forward;
        }
        else
        {
            originPosition = transform.position;
            originDirection = Quaternion.Euler(transform.eulerAngles) * Vector3.forward;
        }


        if (Physics.Raycast(originPosition, originDirection, out hit, reticleMaxDistance))
        {
            reticleOject.transform.position = originPosition + (originDirection * hit.distance);
        }
        else
        {
            reticleOject.transform.position = originPosition + (originDirection * reticleMaxDistance);
        }*/
        if (VRSettings.enabled)
        {
            //originPosition = transform.TransformPoint(InputTracking.GetLocalPosition(VRNode.CenterEye));
            originDirection = InputTracking.GetLocalRotation(VRNode.CenterEye).eulerAngles.normalized;
        }
        else
        {
            //originPosition = transform.localPosition;
            originDirection = Vector3.forward;
        }


        if (Physics.Raycast(Vector3.zero, originDirection, out hit, reticleMaxDistance))
        {
            reticleOject.transform.localPosition = (originDirection * hit.distance);
        }
        else
        {
            reticleOject.transform.localPosition = (originDirection * reticleMaxDistance);
        }
    }
}
