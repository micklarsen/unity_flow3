
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

public class Tile : MonoBehaviour
{
    public Material texture;
    const float maxHeight = 999f;
    public int ActionLevelMax;
    [SerializeField]
    private Vector2 tileSize;
    //public Voronoi Voronoi;

    //private List<Biome> m_biomes = new List<Biome>();

    //// Create delaunay
    //private void Awake()
    //{
    //    Voronoi.VoronoiDiagram
    //}

    // The number of polygons/sites we want
    public int polygonNumber = 15;

    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;

    // DELETE ME: 

    [HideInInspector]
    public List<Vector3> SouthernEdge = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> WesternEdge = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> NorthernEdge = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> EasternEdge = new List<Vector3>();

    public bool StitchedSouth = false;
    public bool StitchedWest = false;
    public bool StitchedNorth = false;
    public bool StitchedEast = false;

    private bool drawTestEdge = false;
    private int i = 0;
    private int j = 0;

    // END DELETE 


    public Vector2 TileSize { get => tileSize; set => tileSize = value; }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         SpawnSphereAtSideCenters();
    //     }

    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         drawTestEdge = !drawTestEdge;
    //     }

    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         i++;
    //     }

    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         j++;
    //     }
    // }

    private void SpawnSphereAtSideCenters()
    {
        foreach (KeyValuePair<Vector2f, Site> site in sites)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            float scale = 5.0f;
            s.transform.localScale = new Vector3(scale, scale, scale);
            s.transform.parent = transform;

            var x = site.Value.x / 2 + transform.position.x;
            var z = site.Value.y / 2 + transform.position.z;
            var pos = new Vector3(x, GetYCoordinate(x, z), z);
            s.transform.position = pos;
        }
    }

    private float GetYCoordinate(float x, float z)
    {
        Ray ray = new Ray(new Vector3(x, maxHeight, z), Vector3.down);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, maxHeight + 1);
        return hit.point.y;
    }

    void Awake()
    {
        // Create your sites (lets call that the center of your polygons)
        List<Vector2f> points = CreateRandomPoint();

        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0, 0, TileSize.x, TileSize.y);

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points, bounds, 5);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;

        //DisplayVoronoiDiagram();
        ApplyTexture();
        Dispose();
    }

    private void ApplyTexture()
    {
        this.GetComponent<Renderer>().material = texture;

    }

    private List<Vector2f> CreateRandomPoint()
    {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++)
        {
            points.Add(new Vector2f(UnityEngine.Random.Range(0, TileSize.x), UnityEngine.Random.Range(0, TileSize.y)));
        }

        return points;
    }

    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D((int)TileSize.x, (int)TileSize.y);
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        this.GetComponent<Renderer>().material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public void UpdateMesh()
    {
        Debug.LogWarning("Update mesh: Not Implemented yet");
    }

    public void OnDrawGizmos()
    {
        if (drawTestEdge)
        {
            if (i > sites.Count) i = 0;
            var site = sites.ElementAt(i).Value;
            if (j > site.Edges.Count) j = 0;
            var edge = site.Edges.ElementAt(j);

            var x0 = edge.ClippedEnds[LR.LEFT].x / 2 + transform.position.x;
            var y0 = edge.ClippedEnds[LR.LEFT].y / 2 + transform.position.z;

            var from = new Vector3
            (
                x0,
                GetYCoordinate(x0, y0),
                y0
            );

            var x1 = edge.ClippedEnds[LR.RIGHT].x / 2 + transform.position.x;
            var y1 = edge.ClippedEnds[LR.RIGHT].y / 2 + transform.position.z;

            var to = new Vector3
            (
                x1,
                GetYCoordinate(x1, y1),
                y1
            );

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(from, 12);
            Gizmos.DrawWireSphere(to, 10);

            Gizmos.DrawLine(from, to);
        }
    }

    #region dispose
    void Dispose()
    {
        Debug.Log("Should dispose mesh, triangles, uvs, other?");
    }
    #endregion
}