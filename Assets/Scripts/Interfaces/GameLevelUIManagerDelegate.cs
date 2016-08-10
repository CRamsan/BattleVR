/// <summary>
/// This delegate declares all the methods that will be called after selecting any of the UI elements.
/// </summary>
public interface GameLevelUIManagerDelegate
{
    // Pause menu delegate methods
    void OnPauseMenuResumeSelected();
    void OnPauseMenuQuitSelected();
    void OnPauseMenuConfirmBackSelected();
    void OnPauseMenuConfirmQuitSelected();

    // TeamSelect and ShipConfig menu delegate methods
    void OnTeamSelectMenuBlueSelected();
    void OnTeamSelectMenuRedSelected();
    void OnShipConfigMenuFigtherSelected();
    void OnShipConfigMenuFrigateSelected();
}