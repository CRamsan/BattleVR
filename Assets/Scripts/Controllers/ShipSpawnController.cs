using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/// <summary>
/// This controller provides logic to spawn AI ships after a specific time interval.
/// </summary>
[RequireComponent(typeof(TeamController))]
public class ShipSpawnController : MonoBehaviour
{
    public GameObject aiShipPrefab;
    public float timeWait = 10;

    public int figtherPerSquad = 4;
    public int frigatePerSquad = 10;
    public int destroyerPerSquad = 1;
    public int maxFigtherPerSquad = 8;
    public int maxFrigatePerSquad = 20;
    public int maxDestroyerPerSquad = 2;

    private int currentFigtherPerSquad;
    private int currentFrigatePerSquad;
    private int currentDestroyerPerSquad;

    private int teamCount;
    private int maxTeamCount;
    private TeamController teamController;
    private GameLevelSceneManager.TEAMTAG team;

    // Use this for initialization
    void Start()
    {
        teamController = transform.GetComponentInChildren<TeamController>();
        team = teamController.team;
        maxTeamCount = maxFigtherPerSquad + maxFrigatePerSquad + maxDestroyerPerSquad;

        if (ExtendedNetworkManager.isHost)
        {
            StartSpawning();
            GameLevelEventManager.ShipDestroyedEvent += TrackShipDestruction;
        }
    }

    void OnDisable()
    {
        GameLevelEventManager.ShipDestroyedEvent -= TrackShipDestruction;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Start the coroutine to spawn AI ships
    /// </summary>
    private void StartSpawning()
    {
        StartCoroutine(SpawnFunction());
    }

    /// <summary>
    /// Stop the coroutine to spawn AI ships
    /// </summary>
    public void StopSpawning()
    {
        StopCoroutine(SpawnFunction());
    }

    /// <summary>
    /// Private method that will handle spawning an AI ship. This method should only be called 
    /// by the spawn coroutine.
    /// </summary>
    private void SpawnShip()
    {
        if (teamCount >= maxTeamCount)
        {
            throw new UnityException();
        }
        for (int j = 0; j < destroyerPerSquad; j++)
        {
            if (currentDestroyerPerSquad < maxDestroyerPerSquad)
            { SpawnShipOfType(ShipController.ShipType.DESTROYER); }
            else
            { break; }
        }
        for (int j = 0; j < frigatePerSquad; j++)
        {
            if (currentFrigatePerSquad < maxFrigatePerSquad)
            { SpawnShipOfType(ShipController.ShipType.FRIGATE); }
            else
            { break; }
        }
        for (int j = 0; j < figtherPerSquad; j++)
        {
            if (currentFigtherPerSquad < maxFigtherPerSquad)
            { SpawnShipOfType(ShipController.ShipType.FIGHTER); }
            else
            { break; }
        }
    }

    private void TrackShipDestruction(ShipController controller)
    {
        if (controller.GetTeam() == team)
        {
            switch (controller.GetShipType())
            {
                case ShipController.ShipType.FIGHTER:
                    currentFigtherPerSquad--;
                    break;
                case ShipController.ShipType.FRIGATE:
                    currentFrigatePerSquad--;
                    break;
                case ShipController.ShipType.DESTROYER:
                    currentDestroyerPerSquad--;
                    break;
                default:
                    throw new Exception();
            }
        }
    }

    private void SpawnShipOfType(ShipController.ShipType shipType)
    {
        GameObject aiPlayer = Instantiate(aiShipPrefab);
        teamController.RegisterUnit(aiPlayer);
        ShipController controller = aiPlayer.GetComponent<ShipController>();
        controller.SetShipType(shipType);
        controller.SetReadyForGame(true);
        Vector3 spawnPosition = GameLevelSceneManager.instance.GetSpawnPosition(team);
        aiPlayer.transform.position = spawnPosition;
        NetworkServer.Spawn(aiPlayer);
        teamCount++;
        switch (shipType)
        {
            case ShipController.ShipType.FIGHTER:
                currentFigtherPerSquad++;
                break;
            case ShipController.ShipType.FRIGATE:
                currentFrigatePerSquad++;
                break;
            case ShipController.ShipType.DESTROYER:
                currentDestroyerPerSquad++;
                break;
            default:
                throw new Exception();
        }
    }

    /// <summary>
    /// This coroutine will handle spawning a ship and waiting until the next call.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnFunction()
    {
        while (true)
        {
            if (teamCount < maxTeamCount)
            {
                SpawnShip();
            }
            else
            {
                StopSpawning();
            }
            yield return new WaitForSeconds(timeWait);
        }
    }

}
