using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipSpawner : MonoBehaviour {

    public float timeWait = 10;
    public int initialCount = 5;
    public GameObject shipPrefab;
    public int maxTeamCount = 15;
    public string teamTag;

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

    public void StartSpawning()
    {
        StartCoroutine(SpawnFunction());
    }

    public void StopSpawning()
    {
        StopCoroutine(SpawnFunction());
    }

    IEnumerator SpawnFunction()
    {
        while (true)
        {
            if (teamCount < maxTeamCount)
            {
                SpawnShip(shipPrefab, new Vector3(0, 0, 5));
            }
            else
            {
                StopSpawning();
            }
            yield return new WaitForSeconds(timeWait);
        }
    }

    private void SpawnShip(GameObject prefab, Vector3 position)
    {
        if (teamCount >= maxTeamCount)
        {
            throw new UnityException();
        }
        GameObject aiPlayer = Instantiate(prefab);
        aiPlayer.transform.tag = teamTag;
        aiPlayer.transform.position = transform.TransformPoint(position);
        NetworkServer.Spawn(aiPlayer);
        teamCount++;
    }
}
