/// <summary>
/// Delegate that allows you to know when different UI events happen in a GameLevelScene.
/// </summary>
public interface GameLevelSceneManagerDelegate
{
    /// <summary>
    /// This method will be called when the user selects a UI elements that will dismiss the pause menu.
    /// </summary>
    void OnPauseMenuResumeSelected();
    /// <summary>
    /// This method will be called when the user selects a ship type to use.
    /// </summary>
    void OnShipConfigMenuShipSelected();
    /// <summary>
    /// This method will be called when the user selects a team.
    /// </summary>
    /// <param name="teamTag"></param>
    void OnTeamSelectMenuTeamSelected(GameLevelSceneManager.TEAMTAG teamTag);
}