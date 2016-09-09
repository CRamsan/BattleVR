using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This controller will provide the behaviour for AI controlled agents.
/// </summary>
public class AIPlayerController : ShipController
{

    public GameObject targetObject;
    public float safeDistance;
    public float shootDistance;

    private Vector3 targetPosition;

    // Use this for initialization
    void Start()
    {
        SafeInit();
        isAI = true;
        // Lets disable this for now until we can improve the movement system
        // InvokeRepeating("ConfigureTarget", 1, 5);
        if (isReadyForGame)
        {
            RefreshTeamState();
            RefreshShipType();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (targetObject != null)
        {
            targetPosition = targetObject.transform.position;
        }
        else
        {
            if (targetPosition.Equals(Vector3.zero) || Vector3.Distance(transform.position, targetPosition) < 15f)
            {
                targetPosition = new Vector3(Random.Range(-400, 400), Random.Range(-100, 100), Random.Range(-250, 250));
            }
        }

        //Get the normalized vector that points towards our target
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        // Get the direction on our local space
        Vector3 aimDirection = transform.InverseTransformDirection(targetDirection);

        // Only move forward when we are facing our target
        float dForward = (aimDirection.z > 0.2f ? aimDirection.z : 0);

        float dLookUp = aimDirection.y * -1;

        float dLookSide = aimDirection.x;

        float targetDistance = Vector3.Distance(targetPosition, transform.position);

#if UNITY_EDITOR
        Color targetRayColor;
        if (targetObject == null)
            targetRayColor = Color.green;
        else
            targetRayColor = Color.red;
        Debug.DrawLine(transform.position, (transform.position + (targetDirection * targetDistance)), targetRayColor);
#endif

        if (targetDistance <= safeDistance)
        {
            dForward /= 1 + (safeDistance - targetDistance);
        }

        if (targetDistance < shootDistance)
        {
            gunController.PressTriger();
        }

#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.TransformPoint(new Vector3(0, 0, aimDirection.z + (dForward * 5))), Color.blue);
        Debug.DrawLine(transform.position, transform.TransformPoint(new Vector3(0, aimDirection.y - (dLookUp * 5), 0)), Color.green);
        Debug.DrawLine(transform.position, transform.TransformPoint(new Vector3(aimDirection.x + (dLookSide * 5), 0, 0)), Color.red);
#endif

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

    //This nethod will be called when this ship takes some damage
    public override void onDamageReceived(float damage, Vector3 position)
    {
        base.onDamageReceived(damage, position);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
