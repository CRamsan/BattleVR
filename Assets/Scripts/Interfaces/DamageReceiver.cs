/// <summary>
/// This interface allows a controller to do something when they recieve damage.
/// </summary>
public interface DamageReceiver
{
    /// <summary>
    /// This method will be called when damage is taken.
    /// </summary>
    /// <param name="damage"></param>
    void onDamageReceived(float damage);
}