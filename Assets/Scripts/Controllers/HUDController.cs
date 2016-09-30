using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

    public GameObject Reticle;
    public GameObject HitMarker;

	// Use this for initialization
	void Start () {
	
	}

    void OnEnable()
    {
        GameLevelEventManager.PlayerProjectileHitEvent += ShowHitMarker;
    }


    void OnDisable()
    {
        GameLevelEventManager.PlayerProjectileHitEvent -= ShowHitMarker;
    }

    // Update is called once per frame
    void Update () {
	
	}

    void ShowHitMarker()
    {
        Image markerImage = HitMarker.GetComponent<Image>();
        markerImage.GetComponent<CanvasRenderer>().SetAlpha(1f);
        markerImage.CrossFadeAlpha(0f, 1f, false);
    }
}
