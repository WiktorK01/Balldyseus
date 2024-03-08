using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using AStar;

public class PathfindingManager : MonoBehaviour
{
    public Tilemap groundTilemap;

    private bool[,] walkableMap;
    private bool[,] walkableMapReference;

    void Start()
    {
        InitializeReferenceWalkableMap();
        InitializeWalkableMap();
    }

    //starting function to initialize the reference map
    void InitializeReferenceWalkableMap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        TileBase[] allTiles = groundTilemap.GetTilesBlock(bounds);

        walkableMapReference = new bool[bounds.size.y, bounds.size.x];

        for (int x = bounds.xMin; x < bounds.size.x; x++)
        {
            for (int y = bounds.yMin; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    walkableMapReference[y, x] = true; // Walkable
                }
                else
                {
                    walkableMapReference[y, x] = false; // Non-walkable
                }
            }
        }
    }
   
    //starting function to initialize the walkable map
    void InitializeWalkableMap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        TileBase[] allTiles = groundTilemap.GetTilesBlock(bounds);

        walkableMap = new bool[bounds.size.y, bounds.size.x];

        for (int x = bounds.xMin; x < bounds.size.x; x++)
        {
            for (int y = bounds.yMin; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    walkableMap[y, x] = true; // Walkable
                }
                else
                {
                    walkableMap[y, x] = false; // Non-walkable
                }
            }
        }
    }
    
    //Resets walkableMap back to it's reference then updates it based on Enemy Positions
    public void UpdateWalkableMap(List<GameObject> enemies)
    {
        Array.Copy(walkableMapReference, walkableMap, walkableMapReference.Length);

        foreach (var enemy in enemies)
        {
            Vector3 worldPosition = enemy.transform.position;
            Vector3Int cellPosition = groundTilemap.WorldToCell(worldPosition);
            walkableMap[cellPosition.y, cellPosition.x] = false; // Mark as unwalkable
        }

        foreach (var obstacle in FindObjectsOfType<Obstacle>())
        {
            Vector3 worldPosition = obstacle.transform.position;
            Vector3Int cellPosition = groundTilemap.WorldToCell(worldPosition);
            walkableMap[cellPosition.y, cellPosition.x] = false; // Mark as unwalkable
        }
    }

    //Returns whether or not a tile is walkable
    public bool isTileWalkable(int x, int y){
        return walkableMap[y, x];
    }

    // Unity Gizmo to draw the walkable tiles
    void OnDrawGizmos()
    {
        if (groundTilemap != null && walkableMap != null)
        {
            BoundsInt bounds = groundTilemap.cellBounds;

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    Vector3Int localPlace = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                    Vector3 worldPlace = groundTilemap.CellToWorld(localPlace);

                    if (walkableMap[y, x]) 
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(worldPlace, new Vector3(.5f, .5f, .5f));
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(worldPlace, new Vector3(.5f, .5f, .5f));
                    }
                }
            }
        }
    }

    public bool[,] GetWalkableMap()
    {
        return walkableMap;
    }

    public void SetTileWalkability(Vector3Int cellPosition, bool isWalkable)
    {
        if (cellPosition.y >= 0 && cellPosition.y < walkableMap.GetLength(0) &&
            cellPosition.x >= 0 && cellPosition.x < walkableMap.GetLength(1))
        {
            walkableMap[cellPosition.y, cellPosition.x] = isWalkable;
        }
    }
}