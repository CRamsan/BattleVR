using UnityEngine;
using System.Collections;

public class TeamController : MonoBehaviour {

    public GameLevelSceneManager.TEAMTAG team;

	// Use this for initialization
	void Start () {
        GameLevelSceneManager.instance.RegisterCapitalShip(team, gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
