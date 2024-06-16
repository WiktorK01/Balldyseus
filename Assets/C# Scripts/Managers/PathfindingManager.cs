using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using AStar;

public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    private bool[,] walkableMap;
    private bool[,] walkableMapReference;

    void Awake()
    {
        GameObject groundTilemapObject = GameObject.FindWithTag("GroundTilemap");

        if (groundTilemapObject != null)
        {
            groundTilemap = groundTilemapObject.GetComponent<Tilemap>();
            if (groundTilemap == null)
            {
                Debug.LogError("Tilemap component not found on the GameObject with the GroundTilemap tag.");
            }
        }
        else
        {
            Debug.LogError("GameObject with the GroundTilemap tag not found.");
        }

        InitializeReferenceWalkableMap();
        InitializeWalkableMap();
    }

    private void InitializeReferenceWalkableMap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        TileBase[] allTiles = groundTilemap.GetTilesBlock(bounds);

        walkableMapReference = new bool[bounds.size.y, bounds.size.x];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int index = (x + bounds.xMin) + (y + bounds.yMin) * bounds.size.x;
                TileBase tile = allTiles[index];
                walkableMapReference[y, x] = tile != null; // Walkable if tile exists
            }
        }
    }

    private void InitializeWalkableMap()
    {
        if (walkableMapReference == null)
        {
            Debug.LogError("Reference walkable map is not initialized.");
            return;
        }

        walkableMap = new bool[walkableMapReference.GetLength(0), walkableMapReference.GetLength(1)];
        Array.Copy(walkableMapReference, walkableMap, walkableMapReference.Length);
    }

    public void UpdateWalkableMap(List<GameObject> enemies)
    {
        if (walkableMapReference == null)
        {
            Debug.LogError("Reference walkable map is not initialized.");
            return;
        }

        if (enemies == null)
        {
            Debug.LogError("Enemies list is null.");
            return;
        }

        Array.Copy(walkableMapReference, walkableMap, walkableMapReference.Length);

        foreach (var enemy in enemies)
        {
            Vector3Int cellPosition = groundTilemap.WorldToCell(enemy.transform.position);
            if (IsWithinBounds(cellPosition))
            {
                walkableMap[cellPosition.y, cellPosition.x] = false; // Mark as unwalkable
            }
        }

        foreach (var obstacle in FindObjectsOfType<Obstacle>())
        {
            Vector3Int cellPosition = groundTilemap.WorldToCell(obstacle.transform.position);
            if (IsWithinBounds(cellPosition))
            {
                walkableMap[cellPosition.y, cellPosition.x] = false; // Mark as unwalkable
            }
        }
    }

    private bool IsWithinBounds(Vector3Int position)
    {
        return position.y >= 0 && position.y < walkableMap.GetLength(0) && position.x >= 0 && position.x < walkableMap.GetLength(1);
    }

    public bool IsTileWalkable(int x, int y)
    {
        if (IsWithinBounds(new Vector3Int(x, y, 0)))
        {
            return walkableMap[y, x];
        }
        return false;
    }

    public bool[,] GetWalkableMap()
    {
        return walkableMap;
    }

    public void SetTileWalkability(Vector3Int cellPosition, bool isWalkable)
    {
        if (IsWithinBounds(cellPosition))
        {
            walkableMap[cellPosition.y, cellPosition.x] = isWalkable;
        }
    }

    // CHECKERS

    public bool CheckIfTileIsWalkable(int x, int y)
    {
        bool[,] walkableMap = GetWalkableMap();
        if (walkableMap == null || y >= walkableMap.GetLength(0) || x >= walkableMap.GetLength(1) || y < 0 || x < 0)
        {
            Debug.LogError("Invalid walkable map data or out of bounds access attempted.");
            return false;
        }
        return walkableMap[y, x] 
        && !CheckIfPositionHasNonPhysicalUnwalkableObstacle(new Vector3(x, y, 0))
        && !CheckIfPositionHasPhysicalUnwalkableObstacle(new Vector3(x, y, 0))  
        && GetObjectWithHealthAtPosition(new Vector3(x, y, 0)) == null;
    }

    public bool CheckIfPositionHasNonPhysicalUnwalkableObstacle(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position);
        foreach (var hit in hits)
        {
            ObstacleTriggerUnwalkable obstacle = hit.GetComponent<ObstacleTriggerUnwalkable>();
            if (obstacle != null)
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckIfPositionHasPhysicalUnwalkableObstacle(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position);
        foreach (var hit in hits)
        {
            ObstaclePhysicalUnwalkable obstacle = hit.GetComponent<ObstaclePhysicalUnwalkable>();
            if (obstacle != null)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject GetObjectWithHealthAtPosition(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position);
        foreach (var hit in hits)
        {
            EnemyProperties enemyProperties = hit.GetComponent<EnemyProperties>();
            if (enemyProperties != null)
            {
                Debug.Log("Object with health found at position: " + position);
                return hit.gameObject;
            }
        }
        return null;
    }

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

                    Gizmos.color = walkableMap[y, x] ? Color.green : Color.red;
                    Gizmos.DrawCube(worldPlace, new Vector3(0.5f, 0.5f, 0.5f));
                }
            }
        }
    }
}