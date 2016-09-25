using UnityEngine;

/// <summary>
/// This interface allows a controller to do something when they recieve damage.
/// </summary>
public interface DamageReceiver
{
    /// <summary>
    /// This method will be called when damage is taken.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="location"></param>
    void OnDamageReceived(float damage, Vector3 location);
}