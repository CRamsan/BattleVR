using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

    public GameObject Reticle;
    public GameObject HitMarker;

    private Image markerImage;
    // Use this for initialization
    void Start () {
	
	}

    void OnEnable()
    {
        GameLevelEventManager.PlayerProjectileHitEvent += ShowHitMarker;
        markerImage = HitMarker.GetComponent<Image>();
        markerImage.GetComponent<CanvasRenderer>().SetAlpha(0.01f);
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
        markerImage.GetComponent<CanvasRenderer>().SetAlpha(1f);
        markerImage.CrossFadeAlpha(0.01f, 1f, false);
    }
}
