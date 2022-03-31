//  ChunkRenderer.cs - Component for handling rendering of the chunk mesh.
//  Some functions used from the following source:
//  Accessible at:  https://web.archive.org/web/20150318013329/http://alexstv.com/index.php/posts/unity-voxel-block-tutorial

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
    //  REFERENCES:
    ChunkData       ChunkData;
    MeshFilter      MeshFilter;
    MeshCollider    MeshCollider;
    Mesh            Mesh;

    //  FUNCTIONS:
    //  Sets component references and mesh when script is loaded.
    private void Awake()
    {
        MeshFilter      = GetComponent<MeshFilter>();
        MeshCollider    = GetComponent<MeshCollider>();
        Mesh            = MeshFilter.mesh;
    }

    //  Sets chunk data with new data.
    public void SetChunkData(ChunkData data)
    {
        ChunkData = data;
    }

    //  Updates chunk mesh with new data.
    public void UpdateChunk(MeshHandler data)
    {
        RenderMesh(data);
    }

    //  Clears current mesh, calculates mesh vertices, triangles, UVs and normals to render the chunk mesh.
    //  Additionally creates separate collision mesh, as some voxel such as water don't generate a collider.
    private void RenderMesh(MeshHandler meshData)
    {
        //  Clears current mesh.
        Mesh.Clear();

        //  Adds a submesh to the mesh, for the water mesh.
        Mesh.subMeshCount = 2;

        //  Adds vertices from both the primary and water mesh.
        Mesh.vertices = meshData.Vertices.Concat(meshData.WaterMesh.Vertices).ToArray();

        //  Gets mesh triangles for both primary and water mesh.
        Mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
        Mesh.SetTriangles(meshData.WaterMesh.Triangles.Select(val => val + meshData.Vertices.Count).ToArray(), 1);

        //  Adds water mesh UVs to the mesh UVs.
        Mesh.uv = meshData.UVs.Concat(meshData.WaterMesh.UVs).ToArray();

        //  Calculates normals for the mesh.
        Mesh.RecalculateNormals();

        //  Resets collision mesh.
        MeshCollider.sharedMesh = null;

        //  Calculates collision mesh vertices, triangles and normals, then sets this new mesh to the collision mesh.
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
        //  Ensure application is running and that there is chunk data to draw before enabling selection.
        if (Application.isPlaying && ChunkData != null)
        {
            //  Draws green cubes around selected editor chunk, or a wire cube around non-selected chunks.
            if (Selection.activeObject == gameObject)
            {
                //  Transparent green
                Gizmos.color    = new Color(0.0f, 1.0f, 0.0f, 0.2f);

                //  Draws cube at the center of the chunk, using the chunks size and width.
                Gizmos.DrawCube
                (
                    transform.position + new Vector3(ChunkData.Width / 2.0f - 0.5f, ChunkData.Height / 2.0f - 0.5f, ChunkData.Width / 2.0f - 0.5f),
                    new Vector3(ChunkData.Width, ChunkData.Height, ChunkData.Width)
                );
            }
            else
            {
                Gizmos.color    = Color.cyan;

                //  Draws wire cube at the center of the chunk, using the chunks size and width.
                Gizmos.DrawWireCube
                (
                    transform.position + new Vector3(ChunkData.Width / 2.0f - 0.5f, ChunkData.Height / 2.0f - 0.5f, ChunkData.Width / 2.0f - 0.5f),
                    new Vector3(ChunkData.Width, ChunkData.Height, ChunkData.Width)
                );
            }
        }
    }
#endif
}