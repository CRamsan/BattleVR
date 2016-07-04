using UnityEngine;
using System.Collections;

public class DebugInputAnalyzer : MonoBehaviour {

    private string buttonState;
    private string axisState;

    // Use this for initialization
    void Start () {
	
	}

    void OnGUI()
    {
        GUI.Box(new Rect(10, 75, 100, 90), buttonState);
        GUI.Box(new Rect(115, 75, 100, 90), axisState);
    }

    // Update is called once per frame
    void Update () {
        buttonState = "";
        for (int i = 0; i <= 20; i++)
        {
            if (Input.GetButtonUp("CONTROLLER_BUTTON_" + i))
            {
                buttonState += "Button " + i + "\n";
            }
        }

        axisState = "";
        for (int i = 1; i <= 15; i++)
        {
            float axis = Mathf.Abs(Input.GetAxis("CONTROLLER_AXIS_" + i));
            if (axis > 0.1f)
            {
                axisState += i + " - " + axis + "\n";
            }
        }
    }
}
