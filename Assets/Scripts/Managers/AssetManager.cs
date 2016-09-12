using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class AssetManager : MonoBehaviour
{
    public enum ASSET
    {
        NONE, FIGHTER_LODGROUP, FRIGATE_LODGROUP, TEAM_RED_MATERIAL, TEAM_BLUE_MATERIAL
    }

    private GameObject fighterLODGroup;
    private GameObject frigateLODGroup;

    private Material redTeamMaterial;
    private Material blueTeamMaterial;

    public static AssetManager instance;

    private void Init()
    {
        fighterLODGroup = ((GameObject)Resources.Load("Fighter1"));
        frigateLODGroup = ((GameObject)Resources.Load("Fighter1"));
        redTeamMaterial = (Material)Resources.Load("TeamRed");
        blueTeamMaterial = (Material)Resources.Load("TeamBlue");
    }

    public void Start()
    {
        Init();
        Assert.IsNull(instance);
        instance = this;
    }

    public void Stop()
    {
        Assert.IsNotNull(instance);
        instance = null;
    }

    public Object GetAsset(ASSET resource)
    {
        switch (resource)
        {
            case ASSET.FIGHTER_LODGROUP:
                return fighterLODGroup;
            case ASSET.FRIGATE_LODGROUP:
                return frigateLODGroup;
            case ASSET.TEAM_BLUE_MATERIAL:
                return blueTeamMaterial;
            case ASSET.TEAM_RED_MATERIAL:
                return redTeamMaterial;
            default:
                throw new UnityException();
        }
    }
}