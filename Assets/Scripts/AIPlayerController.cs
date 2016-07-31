using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AIPlayerController : ShipController{

    public GameObject targetObject;

    private GameObject targetReticle;

    private float counter;
    public float safeDistance;
    public float shootDistance;

    // Use this for initialization
    void Start () {
        Init();
        if (isLocalPlayer)
        {
        }

        targetReticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetReticle.transform.SetParent(transform);
        targetReticle.transform.localPosition = Vector3.forward;
        targetReticle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        isAI = true;
    }

    // Update is called once per frame
    void Update () {
        if (!isServer)
        {
            return;
        }

        counter += Time.deltaTime;

        string enemyTag;
        if (this.tag == "TeamRed")
        {
            enemyTag = "TeamWhite";
        }
        else
        {
            enemyTag = "TeamRed";
        }

        if (counter > 5)
        {
            targetObject = GameObject.FindGameObjectWithTag(enemyTag);
            counter = 0;
        }

        Vector3 targetPosition;
        if (targetObject != null)
        {
            targetPosition = targetObject.transform.position;
        }
        else
        {
            targetPosition = Vector3.zero;
        }

        //Get the normalized vector that points towards our target
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        targetReticle.transform.position = transform.position + targetDirection;

        // Get the direction on our local space
        Vector3 aimDirection = targetReticle.transform.localPosition;
        // Only move forward when we are facing our target
        float dForward = (aimDirection.z > 0 ? aimDirection.z : 0);
        float dLookUp = aimDirection.y * -1;
        if (dLookUp == 0)
        { dLookUp = 1f; }
        float dLookSide = aimDirection.x;
        if (dLookSide == 0)
        { dLookSide = 1f; }
        float targetDistance = (targetPosition - transform.position).magnitude;
        Debug.DrawLine(transform.position, (transform.position + (targetDirection * targetDistance)), Color.cyan);

        if (Mathf.Sqrt((dLookSide*dLookSide) + (dLookUp*dLookSide)) > dForward ||
            (targetDistance < safeDistance))
        { dForward /= 1 + (safeDistance - targetDistance); }

        if (targetDistance < shootDistance || true)
        {
            //if (dForward >= 0.9f)
            {
                gunController.PressTriger();
            }
        }

        Debug.DrawLine(transform.position, transform.TransformPoint(new Vector3(0, 0, aimDirection.z + (dForward * 5))), Color.blue);
        Debug.DrawLine(transform.position, transform.TransformPoint(new Vector3(0, aimDirection.y - (dLookUp * 5), 0)), Color.green);
        Debug.DrawLine(transform.position, transform.TransformPoint(new Vector3(aimDirection.x + (dLookSide * 5), 0, 0)), Color.red);

        Vector3 leftStickVector = new Vector3(0, 0, dForward);
        Vector3 rotationStickVector = new Vector3(dLookUp, dLookSide, 0f);

        HandleInput(leftStickVector, rotationStickVector);
    }
}
