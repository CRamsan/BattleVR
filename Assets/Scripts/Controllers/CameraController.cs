using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR;

/// <summary>
/// This controller provides functionality for the camera in both VR and non-VR enviroments. 
/// </summary>
public class CameraController : MonoBehaviour
{

    public float reticleMaxDistance = 20f;
    public GameObject reticlePrefab;

    private Camera playerCamera;
    private GameObject reticleOject;
    private GameObject lastSelected;
    private RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        reticleOject = Instantiate(reticlePrefab);
        reticleOject.transform.SetParent(transform);
        playerCamera = GetComponent<Camera>();

        // Configure camera
        playerCamera.farClipPlane = 5000;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 originPosition;

        if (VRSettings.enabled)
        {
            originPosition = transform.TransformPoint(InputTracking.GetLocalPosition(VRNode.CenterEye));
        }
        else
        {
            originPosition = transform.position;
        }

        Ray ray3D = new Ray(originPosition, transform.TransformDirection(Vector3.forward));
        bool reticleHit = Physics.Raycast(ray3D, out hit, reticleMaxDistance);

        float reticleDistance = reticleHit ? hit.distance : reticleMaxDistance;
        reticleOject.transform.position = transform.TransformPoint(new Vector3(0, 0, reticleDistance));

        EventSystem eventSystem = EventSystem.current;

        if (eventSystem != null)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }

            if (InputManager.WasActionPressed(InputManager.CONTROLLER_ACTION.SELECT))
            {
                if (eventSystem.currentSelectedGameObject == null)
                {
                    eventSystem.SetSelectedGameObject(lastSelected);
                }
            }
        }

#if UNITY_EDITOR
        if (reticleHit)
            Debug.DrawLine(originPosition, reticleOject.transform.position, Color.cyan);
#endif
    }
}
