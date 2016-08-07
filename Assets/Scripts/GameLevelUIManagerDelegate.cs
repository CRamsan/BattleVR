using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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