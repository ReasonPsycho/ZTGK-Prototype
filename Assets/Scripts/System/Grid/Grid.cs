using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Tile currentTile;

    private void Start()
    {
        gridArray = new Tile[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = new Tile(x * cellSize + transform.position.x + offsetX, y * cellSize + transform.position.z + offsetZ);
                Vector3 from = new Vector3(x * cellSize + transform.position.x + offsetX, 0.0f + transform.position.y, y * cellSize + transform.position.z + offsetZ);
                Debug.DrawLine(from, from + new Vector3(cellSize, 0, 0), Color.red, 100f);

                Debug.DrawLine(from, from + new Vector3(0, 0, cellSize), Color.red, 100f);
            }
        }
    }

    private void Update()
    {

        //highlight the cell the mouse is over
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 point = hit.point - transform.position;
                int indexX = Mathf.FloorToInt(point.x - offsetX / cellSize);
                int indexY = Mathf.FloorToInt(point.z - offsetZ / cellSize);
                print(indexX + " " + indexY);


                currentTile = gridArray[indexX, indexY];
            }
            else
            {
                currentTile = null;
            }


            if (currentTile != null)
            {
                
            }

        



    }

    public bool placeBuilding(int x, int y, GameObject building)
    {
        if (gridArray[x,y].building == null)
        {
            gridArray[x, y].building = Instantiate(
                building,
                new Vector3(x * cellSize + transform.position.x + offsetX, 0.0f + transform.position.y, y * cellSize + transform.position.z + offsetZ),
                Quaternion.identity
            );
            return true;
        }
        return false;
    }

    

    
}
