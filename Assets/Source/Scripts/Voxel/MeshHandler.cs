//  MeshHandler.cs - Handles mesh data as well as adding vertices and triangles to the mesh.
//  Class designed based on the following source:
//  Available at:   https://web.archive.org/web/20150318013329/http://alexstv.com/index.php/posts/unity-voxel-block-tutorial

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CLASS:
public class MeshHandler
{
    //  VARIABLES:
    public List<Vector3>    Vertices            = new List<Vector3>();
    public List<Vector2>    UVs                 = new List<Vector2>();
    public List<int>        Triangles           = new List<int>();

    public List<Vector3>    ColliderVertices    = new List<Vector3>();
    public List<int>        ColliderTriangles   = new List<int>();

    public MeshHandler      WaterMesh;

    //  FUNCTIONS:
    //  Function to add vertex to list and add to collider list if the voxel generates collision.
    public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
    {
        Vertices.Add(vertex);

        //  Adds to collider vertices if necessary.
        if (vertexGeneratesCollider)
        {
            ColliderVertices.Add(vertex);
        }
    }

    //  Generates triangles for each quad of the voxel in a clockwise direction.
    public void AddQuadTriangles(bool quadGeneratesCollider)
    {
        Triangles.Add(Vertices.Count - 4);
        Triangles.Add(Vertices.Count - 3);
        Triangles.Add(Vertices.Count - 2);

        Triangles.Add(Vertices.Count - 4);
        Triangles.Add(Vertices.Count - 2);
        Triangles.Add(Vertices.Count - 1);

        //  Adds triangles to collider.
        if (quadGeneratesCollider)
        {
            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 3);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            ColliderTriangles.Add(ColliderVertices.Count - 1);
        }
    }
}