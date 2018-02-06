using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapDisplay : MonoBehaviour {

    public Renderer mapRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void DrawTexture(Texture2D texture)
    {
        mapRenderer.sharedMaterial.mainTexture = texture;
        mapRenderer.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);
    }

    public void DrawMesh(ChunkMeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawVoxel(ChunkMeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
    }
}
