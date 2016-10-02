using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

/// <summary>
/// Central object that will manage and broker messages between units within a team.
/// </summary>
public class TeamController : MonoBehaviour {

    public GameLevelSceneManager.TEAMTAG team;
    public GameObject capitalShip;
    private List<GameObject> teamUnits;
    private bool gameEnded;

	// Use this for initialization
	void Start () {
        Assert.IsNotNull(capitalShip);
        capitalShip.GetComponentInChildren<CapitalShipController>().SetTeamController(this);
        GameLevelSceneManager.instance.RegisterCapitalShip(team, capitalShip);
        teamUnits = new List<GameObject>();
        gameEnded = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Add this unit to this team so we can track it
    public void RegisterUnit(GameObject newUnit)
    {
        if (!teamUnits.Contains(newUnit))
        {
            ShipController controller = newUnit.GetComponent<ShipController>();
            newUnit.transform.SetParent(transform);
            Assert.IsNotNull(controller);
            if (controller != null)
            {
                controller.SetTeam(team);
                controller.SetTeamController(this);
            }
            // Notify the new unit about all existing units
            foreach (GameObject teamUnit in teamUnits)
            {
                newUnit.GetComponent<ShipController>().OnFriendlyShipSpawned(teamUnit.GetComponent<ShipController>());
            }
            // Now notify the existing units about this new unit
            foreach (GameObject teamUnit in teamUnits)
            {
                teamUnit.GetComponent<ShipController>().OnFriendlyShipSpawned(controller);
            }
            teamUnits.Add(newUnit);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public void OnUnitDestroyed(GameObject unit)
    {
        if (unit.GetComponent<ShipController>() != null)
        {
            Assert.IsTrue(teamUnits.Contains(unit));
            teamUnits.Remove(unit);
            Debug.Log("Ship of team " + team + " was destroyed");
            foreach (GameObject teamUnit in teamUnits)
            {
                teamUnit.GetComponent<ShipController>().OnFriendlyShipDestroyed(unit.GetComponent<ShipController>());
            }
        }
        else if (unit.GetComponent<CapitalShipController>() != null)
        {
            if (!gameEnded)
            {
                Debug.Log("Capital Ship of team " + team + " was destroyed!");
                GameLevelSceneManager.instance.OnCapitalShipDestroyed(team);
            }
            gameEnded = true;
        }
    }

    public void OnGameEnded(bool win)
    {
        foreach (GameObject teamUnit in teamUnits)
        {
            teamUnit.GetComponent<ShipController>().OnGameEnded(win);
        }
    }
}
