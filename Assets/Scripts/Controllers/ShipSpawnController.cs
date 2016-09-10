using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// This controller provides logic to spawn AI ships after a specific time interval.
/// </summary>
[RequireComponent(typeof(TeamController))]
public class ShipSpawnController : MonoBehaviour
{
    public GameObject aiShipPrefab;
    public float timeWait = 10;
    public int maxTeamCount = 15;

    private int teamCount;
    private TeamController teamController;
    private GameLevelSceneManager.TEAMTAG team;

    // Use this for initialization
    void Start()
    {
        teamController = transform.GetComponentInChildren<TeamController>();
        team = teamController.team;

        if (ExtendedNetworkManager.isHost)
        {
            StartSpawning();
        }
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
    private void StopSpawning()
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
        GameObject aiPlayer = Instantiate(aiShipPrefab);
        teamController.RegisterUnit(aiPlayer);
        ShipController controller = aiPlayer.GetComponent<ShipController>();
        controller.SetShipType(ShipController.ShipType.FRIGATE);
        controller.SetReadyForGame(true);
        Vector3 spawnPosition = GameLevelSceneManager.instance.GetSpawnPosition(team);
        aiPlayer.transform.position = spawnPosition;
        NetworkServer.Spawn(aiPlayer);
        teamCount++;
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
