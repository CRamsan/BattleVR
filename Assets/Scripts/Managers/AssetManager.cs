using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class AssetManager : MonoBehaviour
{
    public enum ASSET
    {
        FIGHTER_MODEL, FRIGATE_MODEL, TEAM_RED_MATERIAL, TEAM_BLUE_MATERIAL
    }

    private Mesh fighterModel;
    private Mesh frigateModel;

    private Material redTeamMaterial;
    private Material blueTeamMaterial;

    public static AssetManager instance;

    private void Init()
    {
        frigateModel = ((GameObject)Resources.Load("Fighter2")).GetComponent<MeshFilter>().sharedMesh;
        fighterModel = ((GameObject)Resources.Load("Fighter2")).GetComponent<MeshFilter>().sharedMesh;
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
            case ASSET.FIGHTER_MODEL:
                return fighterModel;
            case ASSET.FRIGATE_MODEL:
                return frigateModel;
            case ASSET.TEAM_BLUE_MATERIAL:
                return blueTeamMaterial;
            case ASSET.TEAM_RED_MATERIAL:
                return redTeamMaterial;
            default:
                throw new UnityException();
        }
    }
}