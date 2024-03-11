using Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public GameObject building;
    public GameObject wall;
    public GameObject wallSide;
    public GameObject floor;

    public Grid grid;

    Animator buildingAnimator;
    

    private void Start()
    {
        wall = Resources.Load<GameObject>("WallPre");

        grid = GameObject.Find("Grid").GetComponent<Grid>();
 
        for (int x = 0; x < grid.gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < grid.gridArray.GetLength(1); z++)
            {
                if (!(x > 40 && x < 60 && z > 40 && z < 60))
                //if (x == 1 && z == 1)
                {
                    grid.gridArray[x, z].Build(Instantiate(wall,
                        new Vector3(x * grid.cellSize + transform.position.x + grid.offsetX + grid.cellSize / 2.0f,
                            0.0f + transform.position.y,
                            z * grid.cellSize + transform.position.z + grid.offsetZ + grid.cellSize / 2.0f),
                        Quaternion.identity, grid.transform), BuildingType.WALL);

                }
                else
                {
                    grid.gridArray[x, z].Build(Instantiate(floor,
                        new Vector3(x * grid.cellSize + transform.position.x + grid.offsetX + grid.cellSize / 2.0f,
                            0.0f + transform.position.y,
                            z * grid.cellSize + transform.position.z + grid.offsetZ + grid.cellSize / 2.0f),
                        Quaternion.identity, grid.transform), BuildingType.FLOOR);

                }
            }
        }

        for (int x = 0; x < grid.gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < grid.gridArray.GetLength(1); z++)
            {
                if (grid.gridArray[x, z].BuildingHandler != null && grid.gridArray[x, z].BuildingHandler.buildingType == BuildingType.WALL)
                {
                    Wall wallComponent = grid.gridArray[x, z].BuildingHandler as Wall;
                    wallComponent.SetWall();
                }
            }
        }
    }

    public IEnumerator buildingConstructionCrtn(float time)
    {
        for (int i = 0; i < 100; i++)
        {
            buildingAnimator.SetFloat("ConstructionProcantage", i / 100.0f);
            yield return new WaitForSeconds(time / 100.0f);
        }
    }

    public bool placeBuilding(List<Tile> tiles, GameObject buildingPrefab) {
        foreach (var tile in tiles) {
            if ( !tile.Vacant ) return false;
        }

        float avgX = 0;
        float avgY = 0;
        foreach (var tile in tiles) {
            if ( tile.BuildingHandler.buildingType == BuildingType.FLOOR )
                tile.BuildingHandler.DestroyBuilding();

            avgX += tile.x;
            avgY += tile.y;
        }

        avgX /= tiles.Count;
        avgY /= tiles.Count;

        var obj = Instantiate(
            buildingPrefab,
            new Vector3(avgX, 0.0f + transform.position.y, avgY),
            Quaternion.identity,
            transform
        );

        foreach (var tile in tiles) {
            tile.Build(obj);
        }

        return true;
    }

    public bool placeBuilding(int x, int y, GameObject building)
    {

        if (grid.gridArray[x, y].BuildingHandler != null)
        {
            if(grid.gridArray[x, y].BuildingHandler.buildingType == BuildingType.FLOOR)
            {
                grid.gridArray[x, y].BuildingHandler.DestroyBuilding();
            }
        }

        if (grid.gridArray[x, y].Building == null)
        {

            GameObject gameObject = Instantiate(building,
                               new Vector3(x * grid.cellSize + transform.position.x + grid.offsetX + grid.cellSize / 2.0f,
                                                  0.0f + transform.position.y, y * grid.cellSize + transform.position.z + grid.offsetZ + grid.cellSize / 2.0f),
                                              Quaternion.identity, transform);
            grid.gridArray[x, y].Build(gameObject);


            StartCoroutine(buildingConstructionCrtn(2.0f));
            Vector3 bldngSize = grid.gridArray[x, y].Building.GetComponentsInChildren<MeshRenderer>()[1].bounds.size;
            grid.gridArray[x, y].Building.transform.localScale = new Vector3(grid.cellSize / bldngSize.x, grid.cellSize / bldngSize.x,
            grid.cellSize / bldngSize.x);
            return true;
        }

        return false;
    }
}
