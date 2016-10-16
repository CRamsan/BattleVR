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
    private bool tempTarget;

    // Use this for initialization
    void Start()
    {
        SafeInit();
        isAI = true;
        if (isReadyForGame)
        {
            RefreshShipType();
            RefreshTeamState();
            // Lets disable this for now until we can improve the movement system
            InvokeRepeating("ConfigureTarget", 1, 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        Vector3 leftStickVector = Vector3.zero, rotationStickVector = Vector3.zero;

        if (!gameEnded)
        {

            if (targetObject != null)
            {
                targetPosition = targetObject.transform.position;

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

                if (targetDistance <= (safeDistance + (GetShipType() == ShipType.DESTROYER ? 200 : 0)))
                {
                    dForward = 0; ///= 1 + (safeDistance - targetDistance);
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

                leftStickVector = new Vector3(0, 0, dForward);
                rotationStickVector = new Vector3(dLookUp, dLookSide, 0f);
            }
        }

        HandleInput(leftStickVector, rotationStickVector);
    }

    /// <summary>
    /// This method will handle deciding if target needs to be updated, and if it does then it will call FindTarget().
    /// </summary>
    private void ConfigureTarget()
    {
        if (targetObject == null || tempTarget)
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
        if (GetShipType() == ShipType.DESTROYER)
        {
            GameObject enemyCapitalShip;
            switch (GetTeam())
            {
                case GameLevelSceneManager.TEAMTAG.RED:
                    enemyCapitalShip = GameLevelSceneManager.instance.GetCapitalShip(GameLevelSceneManager.TEAMTAG.BLUE);
                    break;
                case GameLevelSceneManager.TEAMTAG.BLUE:
                    enemyCapitalShip = GameLevelSceneManager.instance.GetCapitalShip(GameLevelSceneManager.TEAMTAG.RED);
                    break;
                default:
                    throw new UnityException();
            }

            CancelInvoke("ConfigureTarget");
            return enemyCapitalShip;
        }
        else
        {
            GameObject target = null, next = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            List<GameObject> enemyList = new List<GameObject>();
            int i = 0;
            while (i < hitColliders.Length)
            {
                Transform parentTransform = hitColliders[i].gameObject.transform.parent;
                // We need this because the collider from each ship is a child of the GameObject.
                if (parentTransform != null)
                {
                    next = parentTransform.gameObject;
                    if (next.CompareTag("Player"))
                    {
                        if (next.GetComponent<ShipController>().GetTeam() != this.GetTeam())
                        {
                            enemyList.Add(next);
                        }
                    }
                }
                i++;
            }
            if (enemyList.Count > 0)
            {
                target = enemyList[0];
            }
            // For now just return the first element found. Ideally the enemy we are going to target is the one that is closest to us.

            tempTarget = target == null;
            if (tempTarget)
            {
                //If no target is founf just approach the enemy Capital Ship
                GameObject enemyCapitalShip;
                switch (GetTeam())
                {
                    case GameLevelSceneManager.TEAMTAG.RED:
                        enemyCapitalShip = GameLevelSceneManager.instance.GetCapitalShip(GameLevelSceneManager.TEAMTAG.BLUE);
                        break;
                    case GameLevelSceneManager.TEAMTAG.BLUE:
                        enemyCapitalShip = GameLevelSceneManager.instance.GetCapitalShip(GameLevelSceneManager.TEAMTAG.RED);
                        break;
                    default:
                        throw new UnityException();
                }
                target = enemyCapitalShip;
                CancelInvoke("ConfigureTarget");
                InvokeRepeating("ConfigureTarget", 1, 1);
            }
            return target;
        }
    }

    //This nethod will be called when this ship takes some damage
    public override void OnDamageReceived(float damage, Vector3 position)
    {
        base.OnDamageReceived(damage, position);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Recieve this message when the ends by either winning or losing
    /// </summary>
    /// <param name="win"></param>
    public override void OnGameEnded(bool win)
    {
        gameEnded = true;
    }
}
