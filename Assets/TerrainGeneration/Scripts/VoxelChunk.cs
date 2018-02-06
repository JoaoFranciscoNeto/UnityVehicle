using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

[ExecuteInEditMode]
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
        col = GetComponent<MeshCollider>();
        world = worldGO.GetComponent("World") as World;

        bounds = new Bounds(transform.position, Vector3.one * chunkSize);

        world.RequestMeshData(new Vector2(chunkX, chunkZ), OnMeshDataReceived);
        SetVisible(false);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateVisibility(GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    void OnMeshDataReceived(ChunkMeshData chunkMeshData)
    {
        mesh = chunkMeshData.CreateMesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        col.sharedMesh = mesh;

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
