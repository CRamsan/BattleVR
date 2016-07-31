using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipSpawner : MonoBehaviour {

    public float timer = 10;
    public int initialCount = 5;
    public GameObject shipPrefab;
    public string teamTag;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < initialCount; i++)
        {
            GameObject aiPlayer = Instantiate(shipPrefab);
            aiPlayer.transform.tag = teamTag;
            aiPlayer.transform.position = transform.TransformPoint(new Vector3(0,05));
            NetworkServer.Spawn(aiPlayer);
        }
        //StartCoroutine(SpawnFunction());
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    IEnumerator SpawnFunction()
    {
        while (true)
        {
            GameObject aiPlayer = Instantiate(shipPrefab);
            aiPlayer.transform.tag = teamTag;
            aiPlayer.transform.position = transform.TransformPoint(new Vector3(0, 05));
            NetworkServer.Spawn(aiPlayer);
            yield return new WaitForSeconds(initialCount);
        }
    }
}
