using UnityEngine;
/// <summary>
/// This delegate declares the methods that will be called by the GunController.
/// </summary>
public interface GunControllerDelegate
{
    /// <summary>
    /// This method will be called when the GunController fires a projectile.
    /// </summary>
    /// <param name="origin"></param>
    void onShootProjectile(Vector3 origin);
    /// <summary>
    /// This method will be called when the GunController starts the reloading process.
    /// </summary>
    void onStartReloading();
}