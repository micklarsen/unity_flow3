using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    private GameObject tilePrefab;
    private Tile tile;

    //private List<GameObject> tiles = new List<GameObject>();
    private Dictionary<Vector2, GameObject> tiles = new Dictionary<Vector2, GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
        tile = tilePrefab.GetComponent<Tile>();

        SpawnTile();
        SpawnTile(250, 0);
        SpawnTile(0, 250);
        SpawnTile(250, 250);

        //SpawnTile(500, 0);
        //SpawnTile(-250, 0);
        //SpawnTile(0, 250);
        //SpawnTile(0, -250);
        //SpawnTile(0, -500);

        //        AlignBoundaryEdges();
        
    }

    private void SpawnTile() => SpawnTile(0f, 0f, 0f);
    private void SpawnTile(float x, float z) => SpawnTile(x, 0.0f, z);
    private void SpawnTile(float x, float y, float z)
    {
        var newTile = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity, transform);
        
        var tx = newTile.transform.position.x / (tile.TileSize.x / 2);
        var ty = newTile.transform.position.z / (tile.TileSize.y / 2);

        var key = new Vector2(tx, ty);
        
        tiles.Add(key, newTile);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StitchSidesToNeighbour();
        }

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile">The current tile</param>
    /// <param name="side">0 = South, 1 = West, 2 = North, 3 = East</param>
    void someMethod(Tile tile, Vector2 key, int side)
    {
        //Consider having an array of 4 ints to replace the bools (stitched*).
        Vector2 kernel = key;
        Tile neighbourTile;
        List<Vector3> stitchedEdge = new List<Vector3>();

        switch(side)
        {
            case 0: // south
                if (tile.StitchedSouth) return;
                
                kernel += new Vector2(0, -1);
                neighbourTile = GetNeighbourTile(kernel);
                if (!neighbourTile) return;
                
                StitchEdges(ref tile.SouthernEdge, ref neighbourTile.NorthernEdge, tile);

                tile.StitchedSouth = true;
                neighbourTile.StitchedNorth = true;

                neighbourTile?.gameObject.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
                break;
            case 1: // west
                if (tile.StitchedWest) return;
                kernel += new Vector2(-1, 0);
                neighbourTile = GetNeighbourTile(kernel);
                if (!neighbourTile) return;

                StitchEdges(ref tile.WesternEdge, ref neighbourTile.EasternEdge, tile);

                tile.StitchedWest = true;
                neighbourTile.StitchedEast = true;

                neighbourTile?.gameObject.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
                break;
            case 2: // north
                if (tile.StitchedNorth) return;
                kernel += new Vector2(0, 1);
                neighbourTile = GetNeighbourTile(kernel);
                if (!neighbourTile) return;

                StitchEdges(ref tile.NorthernEdge, ref neighbourTile.SouthernEdge, tile);

                tile.StitchedNorth = true;
                neighbourTile.StitchedSouth = true;
        
                neighbourTile?.gameObject.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
                break;
            case 3: // east 
                if (tile.StitchedEast) return;
                kernel += new Vector2(1, 0);
                neighbourTile = GetNeighbourTile(kernel);
                if (!neighbourTile) return;

                StitchEdges(ref tile.EasternEdge, ref neighbourTile.WesternEdge, tile);
             
                tile.StitchedEast = true;
                neighbourTile.StitchedWest = true;
        
                neighbourTile?.gameObject.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
                break; 
        }

        tile.gameObject.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();


    }

    private Tile GetNeighbourTile(Vector2 kernel) => tiles.ContainsKey(kernel) ? tiles[kernel]?.GetComponent<Tile>() : null;
//    {
//        if (tiles.ContainsKey(kernel))
//        {
//            return tiles[kernel]?.GetComponent<Tile>();
//        }
//return null;

//return tiles.ContainsKey(kernel) ? tiles[kernel]?.GetComponent<Tile>() : null;
//    }

    private void AlignBoundaryEdges()
    {
        foreach (KeyValuePair<Vector2, GameObject> t in tiles)
        {
            var ti = t.Value.GetComponent<Tile>();
            for (int i = 0; i < 4; i++)
            {
                someMethod(ti, t.Key, i);
            }

            

            //if (tiles.ContainsKey(t.Key + right))
            //{
            //    var nt = tiles[t.Key + right];
            //    var ntile = nt.GetComponent<Tile>();
            //    if (ntile.StitchedWest) continue;
                
            //    var ti = t.Value.GetComponent<Tile>();
            //    var stitchedEdge = StitchEdges(ti.EasternEdge, ntile.WesternEdge);

            //    ti.EasternEdge = stitchedEdge;
            //    ntile.WesternEdge = stitchedEdge;

            //    ti.StitchedEast = true;
            //    ntile.StitchedWest = true;

            //    // Find no ref way of updating mesh and still DRY 
            //    t.Value.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //    nt.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //}

            //if (tiles.ContainsKey(t.Key + left))
            //{
            //    var nt = tiles[t.Key + left];
            //    var ntile = nt.GetComponent<Tile>();
            //    if (ntile.StitchedEast) continue;
             
            //    var ti = t.Value.GetComponent<Tile>();
            //    var stitchedEdge = StitchEdges(ti.WesternEdge, ntile.EasternEdge);

            //    ti.WesternEdge = stitchedEdge;
            //    ntile.EasternEdge = stitchedEdge;

            //    ti.StitchedWest = true;
            //    ntile.StitchedEast = true;

            //    // Find no ref way of updating mesh and still DRY 
            //    t.Value.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //    nt.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //}

            //if (tiles.ContainsKey(t.Key + up))
            //{
            //    var nt = tiles[t.Key + up];
            //    var ntile = nt.GetComponent<Tile>();
            //    if (ntile.StitchedSouth) continue;
             
            //    var ti = t.Value.GetComponent<Tile>();
            //    var stitchedEdge = StitchEdges(ti.NorthernEdge, ntile.SouthernEdge);

            //    ti.NorthernEdge = stitchedEdge;
            //    ntile.SouthernEdge = stitchedEdge;

            //    ti.StitchedNorth = true;
            //    ntile.StitchedSouth = true;

            //    // Find no ref way of updating mesh and still DRY 
            //    t.Value.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //    nt.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //}

            //if (tiles.ContainsKey(t.Key + down))
            //{
            //    var nt = tiles[t.Key + down];
            //    var ntile = nt.GetComponent<Tile>();
            //    if (ntile.StitchedNorth) continue;

            //    var ti = t.Value.GetComponent<Tile>();
            //    var stitchedEdge = StitchEdges(ti.SouthernEdge, ntile.NorthernEdge);

            //    ti.SouthernEdge = stitchedEdge;
            //    ntile.NorthernEdge = stitchedEdge;

            //    ti.StitchedSouth = true;
            //    ntile.StitchedNorth = true;

            //    // Find no ref way of updating mesh and still DRY 
            //    t.Value.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //    nt.GetComponent<TileMeshCreator>().UpdateBoundaryVertices();
            //}
        }
    }

    private void StitchEdges(ref List<Vector3> edgeA, ref List<Vector3> edgeB, Tile tile)
    {
        var result = new List<Vector3>();
        if (edgeA.Count != edgeB.Count) Debug.LogError("Edges to be stitched doesn't match in size");

        for (int i = 0; i < edgeA.Count; i++)
        {
            result.Add((edgeA[i] + edgeB[i]) / 2);
            
            /* DELETE ME */
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.localScale = new Vector3(5f, 5f, 5f);

            s.transform.position = result[i];
            s.transform.parent = tile.gameObject.transform;
            /* DELETE ME END */
        }
        edgeA = result;
        edgeB = result;
    }

    private void StitchSidesToNeighbour()
    {
        foreach (KeyValuePair<Vector2, GameObject> t in tiles)
        {
            Vector2 kernel = t.Key;

            var ti = t.Value.GetComponent<Tile>();
            kernel += new Vector2(-1, 0);
            var neighbourTile = GetNeighbourTile(kernel);
            if (!neighbourTile) continue;

            for (int i = 0; i < neighbourTile.WesternEdge.Count; i++)
            {
                ti.WesternEdge[i] = neighbourTile.EasternEdge[i];
            }
 
            for (int i = 0; i < neighbourTile.NorthernEdge.Count; i++)
            {

                ti.NorthernEdge[i] = neighbourTile.SouthernEdge[i];
            }

            ti.UpdateMesh();
        }
    }
}
