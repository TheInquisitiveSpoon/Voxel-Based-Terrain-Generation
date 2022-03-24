//  ChunkRenderer.cs - Component for handling rendering of the chunk mesh.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//  Ensure required components are attached to the same object.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

//  CLASS:
public class ChunkRenderer : MonoBehaviour
{
    //  VARIABLES:
    ChunkData       ChunkData;
    MeshFilter      MeshFilter;
    MeshCollider    MeshCollider;
    Mesh            Mesh;

    bool            showGizmo   = true;

    //  FUNCTIONS:
    //  Sets component references and mesh when script is loaded.
    private void Awake()
    {
        MeshFilter      = GetComponent<MeshFilter>();
        MeshCollider    = GetComponent<MeshCollider>();
        Mesh            = MeshFilter.mesh;
    }

    //  Sets chunk data with passed value.
    public void SetChunkData(ChunkData data)
    {
        ChunkData = data;
    }

    //  Updates chunk mesh with new data.
    public void UpdateChunk(MeshHandler data)
    {
        RenderMesh(data);
    }

    //  Clears current mesh, calculates mesh vertices, triangles, UV's and normals to render the chunk mesh.
    private void RenderMesh(MeshHandler meshData)
    {
        Mesh.Clear();

        //  Gets mesh vertices.
        Mesh.subMeshCount = 2;
        Mesh.vertices = meshData.Vertices.Concat(meshData.WaterMesh.Vertices).ToArray();

        //  Gets mesh triangles.
        Mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
        Mesh.SetTriangles(meshData.WaterMesh.Triangles.Select(val => val + meshData.Vertices.Count).ToArray(), 1);

        //  Gets mesh UVs
        Mesh.uv = meshData.UVs.Concat(meshData.WaterMesh.UVs).ToArray();
        Mesh.RecalculateNormals();

        //  Handles collider mesh vertices and triangles.
        MeshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.ColliderVertices.ToArray();
        collisionMesh.triangles = meshData.ColliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();
        MeshCollider.sharedMesh = collisionMesh;
    }

//  Only enabled if using unity editor.
#if UNITY_EDITOR
    //  Draws outlines of selected chunks in the scene window if gizmos are enabled.
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            //  Ensure application is running and that there is chunk data before eneabling selection.
            if (Application.isPlaying && ChunkData != null)
            {
                //  Changes draw colour if object is selected.
                if (Selection.activeObject == gameObject)
                {
                    Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.2f);

                    //  Draws gizmo.
                    Gizmos.DrawCube(transform.position + new Vector3(ChunkData.Width / 2.0f, 0.0f, ChunkData.Width / 2.0f),
                        new Vector3(ChunkData.Width, ChunkData.Height, ChunkData.Width));
                }
                else
                {
                    Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.4f);

                    //  Draws gizmo.
                    Gizmos.DrawWireCube(transform.position + new Vector3(ChunkData.Width / 2.0f, 0.0f, ChunkData.Width / 2.0f),
                        new Vector3(ChunkData.Width, ChunkData.Height, ChunkData.Width));
                }
            }
        }
    }
#endif
}