using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour {

    public Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
        {

        }
        else
        {
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }
        float dx = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_RIGHT_X);
        float dz = InputManager.GetAxis(InputManager.CONTROLLER_ANALOG.STICK_LEFT_Y);

        rigidBody.MovePosition(new Vector3(transform.position.x + dx, transform.position.y, transform.position.z + dz));
    }
}
