using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : MonoBehaviour{
    

    private Mesh mesh;
    private MeshCollider col;
    
    public GameObject worldGO;
    private World world;

    public int chunkSize = 16;

    public int chunkX;
    public int chunkY;
    public int chunkZ;

    Bounds bounds;

    // Use this for initialization
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        world = worldGO.GetComponent("World") as World;

        bounds = new Bounds(new Vector3(chunkX, chunkY, chunkZ), Vector3.one * chunkSize);

        world.RequestMapData(OnMapDataReceived);

        SetVisible(false);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void OnMapDataReceived(MapData mapData)
    {

    }

    public void UpdateVisibility (Vector3 viewerPosition)
    {
        float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool visible = viewerDistFromNearestEdge <= InfiniteTerrain.maxViewDistance;
        SetVisible(visible);
    }

    void SetVisible(bool visible)
    {
        GetComponent<MeshRenderer>().enabled = visible;
        GetComponent<MeshCollider>().enabled = visible;
    }
}
