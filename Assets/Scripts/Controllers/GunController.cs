using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

/// <summary>
/// </summary>
public class GunController : MonoBehaviour
{

    public enum GUN_MODE
    {
        IDLE, FIRING, RELOADING
    }

    public GunControllerDelegate gunControllerDelegate;
    public int defaultReserveSize = 100;
    public int defaultMagSize = 25;
    public float rechamberTime = 0.25f;
    public float reloadTime = 0.1f;
    public List<Vector3> projectileOrigins;

    private GUN_MODE firing = GUN_MODE.IDLE;
    private float counter = 0f;
    private int currentMagsize;
    private int currentReserveSize;
    private int originCounter = 0;

    void Start()
    {
        currentMagsize = defaultMagSize;
        currentReserveSize = defaultReserveSize;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (firing == GUN_MODE.FIRING)
        {
            if (counter >= rechamberTime)
            {
                counter = 0f;
                firing = GUN_MODE.IDLE;
            }
        }
        else if (firing == GUN_MODE.RELOADING)
        {
            if (counter >= reloadTime)
            {
                counter = 0f;
                firing = GUN_MODE.IDLE;
                currentMagsize = defaultMagSize;
                currentReserveSize -= defaultMagSize;
            }
        }
    }

    // This method is equivalent to have the trigger pressed.
    public void PressTriger()
    {
#if UNITY_EDITOR
        Assert.IsNotNull(gunControllerDelegate);
        if (gunControllerDelegate == null)
        {
            return;
        }
#endif
        if (firing == GUN_MODE.IDLE)
        {
            if (currentMagsize > 0)
            {
                if (projectileOrigins.Count > 0)
                {
                    gunControllerDelegate.onShootProjectile(projectileOrigins[originCounter++]);
                    if (originCounter >= projectileOrigins.Count)
                    {
                        originCounter = 0;
                    }
                }
                else
                {
                    gunControllerDelegate.onShootProjectile(Vector3.zero);
                }
                firing = GUN_MODE.FIRING;
                currentMagsize--;
            }
            else
            {
                gunControllerDelegate.onStartReloading();
                firing = GUN_MODE.RELOADING;
            }
            counter = 0f;
        }
        else
        {
            if (firing != GUN_MODE.IDLE)
            {
                // We can use this spot to play some sound, you
                // are pressing the trigger but the gun is doing 
                // something right now, maybe firing or reloading
            }
        }
    }

    /// <summary>
    /// Set the values to that of the provided WeaponController.
    /// </summary>
    /// <param name="controller"></param>
    public void SetWeapon(WeaponController controller)
    {
        rechamberTime = controller.rechamberTime;
        reloadTime = controller.reloadTime;
        defaultMagSize = controller.defaultMagazineSize;
        defaultReserveSize = controller.defaultReserveSize;
    }
}
