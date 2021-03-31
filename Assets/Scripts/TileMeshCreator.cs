using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMeshCreator : MonoBehaviour
{
    Mesh m_mesh;

    Vector3[] m_vertices;
    Vector2[] m_uvs;
    int[] m_triangles;

    public int xSize;
    public int zSize;
    [SerializeField]
    private float scale = 50;

    private List<Vector3> m_southernEdge = new List<Vector3>();
    private List<Vector3> m_westernEdge = new List<Vector3>();
    private List<Vector3> m_northernEdge = new List<Vector3>();
    private List<Vector3> m_easternEdge = new List<Vector3>();

    private void Awake()
    {
        m_mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_mesh;

        CreateShape();
       
        UpdateMesh();

        // meh
        transform.localScale = new Vector3(scale, scale, scale);

        FindAllBoundaryVerticesRef();
        //FindAllBoundaryVertices();
        AddRelativePositionToBoundaryVertices();
        SetTileBoundaryVertices();
    }

    private void CreateShape()
    {
        m_vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * UnityEngine.Random.Range(0.2f, 0.4f), z * UnityEngine.Random.Range(0.2f, 0.4f) * 2f);
                m_vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        m_triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                m_triangles[tris + 0] = vert + 0;
                m_triangles[tris + 1] = vert + xSize + 1;
                m_triangles[tris + 2] = vert + 1;
                m_triangles[tris + 3] = vert + 1;
                m_triangles[tris + 4] = vert + xSize + 1;
                m_triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        m_uvs = new Vector2[m_vertices.Length];

        for (int i = 0; i < m_uvs.Length; i++)
        {
            m_uvs[i] = new Vector2(m_vertices[i].x / xSize, m_vertices[i].z / zSize);
        }
    }

    private void UpdateMesh()
    {
        m_mesh.Clear();

        m_mesh.vertices = m_vertices;
        m_mesh.uv = m_uvs;
        m_mesh.triangles = m_triangles;

        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = m_mesh;
    }

    public void UpdateBoundaryVertices()
    {
        var tile = GetComponent<Tile>();
        var maxSize = ((xSize + 1) * (zSize + 1));
        
        int i = 0;
        for (int x0 = 0; x0 <= xSize; x0++)
        {
            m_vertices[x0] = transform.InverseTransformPoint(m_southernEdge[i++]);
        }

        i = 0;
        int tmp = 0;
        for (int y0 = 0; y0 < maxSize; y0 += xSize + 1)
        {
            Debug.Log($"vert: {m_vertices[y0]} at {y0}, edgeV: {tile.WesternEdge[i]} at {i}");
            m_vertices[y0] = transform.InverseTransformPoint(tile.WesternEdge[i++]);
        }

        UpdateMesh();
    }

    private void FindAllBoundaryVerticesRef()
    {
        //var boundaryVertices = new List<Vector3>(m_vertices);
        //boundaryVertices = boundaryVertices.Distinct().ToList();
        var maxSize = ((xSize + 1) * (zSize + 1));

        for (int x0 = 0; x0 <= xSize; x0++)
        {
            m_southernEdge.Add(m_vertices[x0] * scale);
        }

        for (int y0 = 0; y0 < maxSize; y0 += xSize + 1)
        {
            m_westernEdge.Add(m_vertices[y0] * scale);
        }

        for (int x1 = (xSize + 1) * zSize; x1 < maxSize; x1++)
        {
            m_northernEdge.Add(m_vertices[x1] * scale);
        }

        for (int y1 = xSize; y1 < maxSize; y1 += xSize + 1)
        {
            m_easternEdge.Add(m_vertices[y1] * scale);
        }
    }

    private void AddRelativePositionToBoundaryVertices()
    {
        for (int x0 = 0; x0 < xSize + 1; x0++)
        {
            m_southernEdge[x0] += transform.position;
            m_northernEdge[x0] += transform.position;
        }

        for (int y0 = 0; y0 < zSize + 1; y0++)
        {
            m_easternEdge[y0] += transform.position;
            m_westernEdge[y0] += transform.position;
        }
    }

    private void SetTileBoundaryVertices()
    {
        var tile = GetComponent<Tile>();
        tile.SouthernEdge = m_southernEdge;
        tile.WesternEdge = m_westernEdge;
        tile.NorthernEdge = m_northernEdge;
        tile.EasternEdge = m_easternEdge;
    }

    // public void OnDrawGizmos()
    // {
    //     for (int x0 = 0; x0 < xSize; x0++)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawLine(m_southernEdge[x0], m_southernEdge[x0 + 1]);

    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawLine(m_northernEdge[x0], m_northernEdge[x0 + 1]);
    //     }

    //     for (int x0 = 0; x0 < zSize; x0++)
    //     {
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawLine(m_easternEdge[x0], m_easternEdge[x0 + 1]);

    //         Gizmos.color = Color.green;
    //         Gizmos.DrawLine(m_westernEdge[x0], m_westernEdge[x0 + 1]);
    //     }
    // }
}
