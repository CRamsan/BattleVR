using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface GameLevelSceneManagerDelegate
{
    void OnPauseMenuDismissed();
    void OnShipConfigMenuDismissed();
    void OnTeamSelectMenuDismissed(LevelSceneManager.TEAMTAG teamTag);
}