using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// This controller provides logic to spawn AI ships after a specific time interval.
/// </summary>
public class ShipSpawnController : MonoBehaviour {

    public GameObject aiShipPrefab;
    public float timeWait = 10;
    public int maxTeamCount = 15;

    private int teamCount;

    // Use this for initialization
    void Start()
    {
        StartSpawning();
    }
	
	// Update is called once per frame
	void Update ()
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
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    private void SpawnShip(GameObject prefab, Vector3 position)
    {
        if (teamCount >= maxTeamCount)
        {
            throw new UnityException();
        }
        GameObject aiPlayer = Instantiate(prefab);
        aiPlayer.transform.position = transform.TransformPoint(position);
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
                SpawnShip(aiShipPrefab, new Vector3(0, 0, 5));
            }
            else
            {
                StopSpawning();
            }
            yield return new WaitForSeconds(timeWait);
        }
    }

}
