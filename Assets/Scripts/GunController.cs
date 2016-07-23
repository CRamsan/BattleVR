using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// </summary>
public class GunController : MonoBehaviour {

	public float rechamberTime = 0.25f;
	public float reloadTime = 0.1f;
	public int magSize = 25;
    public int reserve = 100;
    public bool endlessAmmo = false;

    private GUN_MODE firing = GUN_MODE.IDLE;
    private float holder = 0f;
	private int currentMag;
    private int currentReserve;
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
        currentReserve = reserve;
	}

	// Update is called once per frame
	void Update ()
    {
        holder += Time.deltaTime;
        if (firing == GUN_MODE.FIRING)
        {
            if (holder >= rechamberTime)
            {
                holder = 0f;
                firing = GUN_MODE.IDLE;
            }
        }
        else if (firing == GUN_MODE.RELOADING)
        {
            if (holder >= reloadTime)
            {
                holder = 0f;
                firing = GUN_MODE.IDLE;
                currentMag = magSize;
                if (!endlessAmmo)
                {
                    currentReserve -= magSize;
                }
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
            if (firing != GUN_MODE.IDLE){
				// We can use this spot to play some sound, you
				// are pressing the trigger but the gun is doing 
				// something right now, maybe firing or reloading
			}
		}
	}

    public void SetWeapon(WeaponController controller)
    {
        rechamberTime = controller.rechamberTime;
        reloadTime = controller.reloadTime;
        magSize = controller.magSize;
    }
}
