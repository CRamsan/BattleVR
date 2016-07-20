using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AIPlayerController : ShipController{

    public Vector3 targetPosition;
    public Vector3 targetDirection;

    private GameObject targetReticle;

    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
            gunController = GetComponent<GunController>();
            gunController.SetGunControllerDelegate(this);
            health = 100f;
        }
        targetPosition = Vector3.zero;
        targetReticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetReticle.transform.SetParent(transform);
        targetReticle.transform.localPosition = Vector3.forward;
        targetReticle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    // Update is called once per frame
    void Update () {
        if (!isServer)
        {
            return;
        }


        targetDirection = (targetPosition - transform.position).normalized;
        targetReticle.transform.position = transform.position + targetDirection;

        Debug.DrawLine(transform.position, (transform.position + (targetDirection * 100)), Color.cyan);

        Vector3 aimDirection = targetReticle.transform.localPosition;
        float dStrafe = 0;
        float dForward = (aimDirection.z > 0 ? aimDirection.z : 0);
        float targetDistance = (targetPosition - transform.position).magnitude;
        if (targetDistance < 25)
        {
            dForward /= (25 - targetDistance);
        }

        float dLookUp = aimDirection.y * -1;
        Debug.DrawLine(transform.position, (new Vector3(transform.position.x, transform.position.y + (dLookUp * 5), transform.position.z)), Color.green);

        float dLookSide = aimDirection.x;
        Debug.DrawLine(transform.position, (new Vector3(transform.position.x + (dLookSide * 5), transform.position.y, transform.position.z)), Color.red);

        float dRotate = 0;

        /*bool fireTrigger = InputManager.IsActionPressed(InputManager.CONTROLLER_ACTION.SHOOT_PRIMARY);
        if (fireTrigger)
        {
            gunController.PressTriger();
        }*/

        Vector3 leftStickVector = new Vector3(dStrafe, 0, dForward);
        Vector3 rotationStickVector = new Vector3(dLookUp, dLookSide, dRotate);

        if (rotationStickVector.magnitude >= 0.2)
        {
            rigidBody.AddRelativeTorque(rotationStickVector * Time.deltaTime * 15);
        }

        if (leftStickVector.magnitude >= 0.2)
        {
            rigidBody.AddRelativeForce(leftStickVector * Time.deltaTime * 2000);
        }
    }
}
