using System.Collections;
using Buildings;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Grid : MonoBehaviour
{
    //number of rows and columns
    public int offsetX = -50;
    public int offsetZ = -50;

    public int width;
    public int height;

    public Tile[,] gridArray;

    //size of each cell
    public float cellSize;

    private void Awake()
    {
        // create the grid
        gridArray = new Tile[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = new Tile(x * cellSize + transform.position.x + offsetX,
                    y * cellSize + transform.position.z + offsetZ, this);
                gridArray[x, y].Index = new Vector2Int(x, y);

                Vector3 from = new Vector3(x * cellSize + transform.position.x + offsetX, 0.0f + transform.position.y,
                    y * cellSize + transform.position.z + offsetZ);
                Debug.DrawLine(from, from + new Vector3(cellSize, 0, 0), Color.red, 100f);

                Debug.DrawLine(from, from + new Vector3(0, 0, cellSize), Color.red, 100f);
            }
        }

    }

    private void Update()
    {

    }

    /*
    void OnApplicationQuit()
    {
        PrefabUtility.UnloadPrefabContents(building);
        PrefabUtility.UnloadPrefabContents(wall);
        PrefabUtility.UnloadPrefabContents(wallSide);
    }
    */

    public Tile GetTile(Vector2Int vector2Int)
    {
        if (vector2Int.x >= 0 && vector2Int.x < width && vector2Int.y >= 0 && vector2Int.y < height)
        {
            return gridArray[vector2Int.x, vector2Int.y];
        }

        return null;
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize + transform.position.x + offsetX + cellSize / 2.0f,
                       0.0f + transform.position.y, y * cellSize + transform.position.z + offsetZ + cellSize / 2.0f);
    }

    public Vector3 GridToWorldPosition(Vector2Int position)
    {
        return GridToWorldPosition(position.x, position.y);
    }

    public Vector2Int WorldToGridPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / cellSize) + 50, Mathf.FloorToInt(position.z / cellSize) + 50);
    }
}