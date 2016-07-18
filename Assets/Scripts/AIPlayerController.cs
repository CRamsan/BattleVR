using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AIPlayerController : ShipController{

    public Vector3 targetPosition;

    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
            gunController = GetComponent<GunController>();
            gunController.SetGunControllerDelegate(this);
            health = 100f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
        {
            return;
        }

        float dStrafe = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.STRAFE);
        float dForward = 1;
        float dLookUp = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.LOOK_UP);
        float dLookSide = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.LOOK_SIDE);
        float dRotate = InputManager.GetAxis(InputManager.CONTROLLER_ACTION.ROTATE);

        bool fireTrigger = InputManager.IsActionPressed(InputManager.CONTROLLER_ACTION.SHOOT_PRIMARY);
        if (fireTrigger)
        {
            gunController.PressTriger();
        }

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
