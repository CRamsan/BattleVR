using UnityEngine;
using System.Collections;

public class SkyboxManager : MonoBehaviour {

    public Material[] skyBoxList;
    private int selectedIndex;
    private Light enviromentLight;

    void Start()
    {
        enviromentLight = (Light)(GameObject.Find("DirectionalLight").GetComponent<Light>());
        setSkyInbox(Random.Range(0, skyBoxList.Length));
    }

    public void setSkyInbox(int index)
    {
        selectedIndex = index;
        RenderSettings.skybox = skyBoxList[selectedIndex];
        Vector3 lightDirection = getDirectionalLightOrientation(selectedIndex);
        enviromentLight.transform.rotation = Quaternion.Euler(lightDirection.x, lightDirection.y, lightDirection.z);
    }

    private Vector3 getDirectionalLightOrientation(int index)
    {
        switch (index)
        {
            default:
                return Vector3.zero;
        }
    }
}
