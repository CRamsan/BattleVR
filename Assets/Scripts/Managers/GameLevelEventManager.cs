using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This class will be the central point for listening for events.
/// </summary>
public class GameLevelEventManager : MonoBehaviour
{
    public delegate void PauseMenuResumeSelected();
    public static event PauseMenuResumeSelected PauseMenuResumeSelectedEvent;

    public delegate void ShipConfigMenuShipSelected(ShipController.ShipType type);
    public static event ShipConfigMenuShipSelected ShipConfigMenuShipSelectedEvent;

    public delegate void TeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG teamTag);
    public static event TeamSelectMenuTeamSelected TeamSelectMenuTeamSelectedEvent;

    public delegate void PauseMenuConfirmQuitSelected();
    public static event PauseMenuConfirmQuitSelected PauseMenuConfirmQuitSelectedEvent;

    public delegate void PlayerProjectileHit();
    public static event PlayerProjectileHit PlayerProjectileHitEvent;

    /// <summary>
    /// This method will be called when the user selects a UI elements that will dismiss the pause menu.
    /// </summary>
    public static void OnPauseMenuResumeSelected()
    {
        Assert.IsNotNull(PauseMenuResumeSelectedEvent);
        if (PauseMenuResumeSelectedEvent != null)
        {
            PauseMenuResumeSelectedEvent();
        }
    }

    /// <summary>
    /// This method will be called when the user selects a ship type to use. The ShipeType is passed as a parameter.
    /// </summary>
    public static void OnShipConfigMenuShipSelected(ShipController.ShipType type)
    {
        Assert.IsNotNull(ShipConfigMenuShipSelectedEvent);
        if (ShipConfigMenuShipSelectedEvent != null)
        {
            ShipConfigMenuShipSelectedEvent(type);
        }
    }

    /// <summary>
    /// This method will be called when the user selects a team.
    /// </summary>
    /// <param name="teamTag"></param>
    public static void OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG teamTag)
    {
        Assert.IsNotNull(TeamSelectMenuTeamSelectedEvent);
        if (TeamSelectMenuTeamSelectedEvent != null)
        {
            TeamSelectMenuTeamSelectedEvent(teamTag);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void OnPauseMenuConfirmQuitSelected()
    {
        Assert.IsNotNull(PauseMenuConfirmQuitSelectedEvent);
        if (PauseMenuConfirmQuitSelectedEvent != null)
        {
            PauseMenuConfirmQuitSelectedEvent();
        }
    }

    public static void OnPlayerProjectileHit()
    {
        Assert.IsNotNull(PlayerProjectileHitEvent);
        if (PlayerProjectileHitEvent != null)
        {
            PlayerProjectileHitEvent();
        }
    }
}
