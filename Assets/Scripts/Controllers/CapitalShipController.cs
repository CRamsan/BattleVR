using UnityEngine;
using System.Collections;
using System;

public class CapitalShipController : MonoBehaviour, DamageReceiver
{
    public float health = 1000;
    private TeamController teamController;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void onDamageReceived(float damage, Vector3 location)
    {
        health -= damage;
        Debug.Log("Capital ship damged: " + health);
        if (health <= 0)
        {
            teamController.OnUnitDestroyed(gameObject);
        }
    }

    /// <summary>
    /// Set the team controller so this ship can communicate
    /// with the rest of the team
    /// </summary>
    /// <param name="controller"></param>
    public void SetTeamController(TeamController controller)
    {
        teamController = controller;
    }
}
