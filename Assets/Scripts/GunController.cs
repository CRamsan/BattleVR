using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// </summary>
public class GunController : MonoBehaviour {

	public float rechamberTime = 0.25f;
	public float reloadTime = 0.1f;
	public int magSize = 100;

    private GUN_MODE firing = GUN_MODE.IDLE;
    private float holder = 0f;
	private int currentMag;
    private GunControllerDelegate controllerDelegate;

	public enum GUN_MODE {
		IDLE, FIRING, RELOADING
	}

    public void SetGunControllerDelegate(GunControllerDelegate controllerDelegate)
    {
        this.controllerDelegate = controllerDelegate;
    }

	void Start(){
		currentMag = magSize;
	}

	// Update is called once per frame
	void Update ()
    {
        if (firing == GUN_MODE.FIRING)
        {
            if (holder >= rechamberTime)
            {
                holder = 0f;
                firing = GUN_MODE.IDLE;
            }
            else
            {
                holder += Time.deltaTime;
            }
        }
        else if (firing == GUN_MODE.RELOADING)
        {
            if (holder >= reloadTime)
            {
                holder = 0f;
                firing = GUN_MODE.IDLE;
                currentMag = magSize;
            }
        }
    }

	// This method is equivalent to have the trigger pressed.
	public void PressTriger() {
		if (firing == GUN_MODE.IDLE) {
			if(currentMag > 0 ){
                controllerDelegate.ShootBullet();
				firing = GUN_MODE.FIRING;
				currentMag--;
			} else {
                controllerDelegate.StartReloading();
                firing = GUN_MODE.RELOADING;
			}
			holder = 0f;
		} else {
			if(firing != GUN_MODE.IDLE){
				// We can use this spot to play some sound, you
				// are pressing the trigger but the gun is doing 
				// something right now, maybe firing or reloading
			}
		}
	}
}
