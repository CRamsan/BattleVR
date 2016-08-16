using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This controller will provide the behaviour for AI controlled agents.
/// </summary>
public class AIPlayerController : ShipController
{

    public GameObject targetObject;
    public float safeDistance;
    public float shootDistance;

    private GameObject targetReticle;

    // Use this for initialization
    void Start()
    {
        Init();

        targetReticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetReticle.transform.SetParent(transform);
        targetReticle.transform.localPosition = Vector3.forward;
        targetReticle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        isAI = true;
        InvokeRepeating("ConfigureTarget", 1, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
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

        if (Mathf.Sqrt((dLookSide * dLookSide) + (dLookUp * dLookSide)) > dForward ||
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

    /// <summary>
    /// This method will handle deciding if target needs to be updated, and if it does then it will call FindTarget().
    /// </summary>
    private void ConfigureTarget()
    {
        if (targetObject == null)
        {
            targetObject = FindTarget(300f);
        }
    }

    /// <summary>
    /// This method will use Physics.OverlapSphere to find an enemy within the provided radius.
    /// This method will return null when no valid target was found.
    /// </summary>
    /// <param name="radius"></param>
    /// <returns>The selected target</returns>
    private GameObject FindTarget(float radius)
    {
        GameObject target = null, next = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> enemyList = new List<GameObject>();
        int i = 0;
        while (i < hitColliders.Length)
        {
            next = hitColliders[i].gameObject;
            if (next.CompareTag("Player"))
            {
                if (next.GetComponent<ShipController>().GetTeam() != this.GetTeam())
                {
                    enemyList.Add(next);
                }
            }
            i++;
        }
        if (enemyList.Count > 0)
        {
            target = enemyList[0];
        }
        // For now just return the first element found. Ideally the enemy we are going to target is the one that is closest to us.
        return target;
    }
}
