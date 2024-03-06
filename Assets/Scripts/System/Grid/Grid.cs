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

    public Tile currentTile;
    private GameObject currentTileHighlight;

    public GameObject building;
    public GameObject wall;
    public GameObject wallSide;
    
    Animator buildingAnimator;

    public bool buildingMode = false;
    private void Start()
    {
      //  building = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Building/Building.prefab") as GameObject; THIS WHY BAD!!
       // wall = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Wall/WallPre.prefab") as GameObject;
       // wallSide = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Wall/WallSide.prefab") as GameObject;/ buildingAnimator = building.GetComponentInChildren<Animator>();
        
        gridArray = new Tile[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = new Tile(x * cellSize + transform.position.x + offsetX,
                    y * cellSize + transform.position.z + offsetZ,this);
                gridArray[x, y].Index = new Vector2Int(x, y);

                Vector3 from = new Vector3(x * cellSize + transform.position.x + offsetX, 0.0f + transform.position.y,
                    y * cellSize + transform.position.z + offsetZ);
                Debug.DrawLine(from, from + new Vector3(cellSize, 0, 0), Color.red, 100f);

                Debug.DrawLine(from, from + new Vector3(0, 0, cellSize), Color.red, 100f);
            }
        }

        currentTileHighlight = GameObject.CreatePrimitive(PrimitiveType.Plane);
        currentTileHighlight.transform.position = new Vector3(100, 100, 100);
        currentTileHighlight.GetComponent<Renderer>().material.color = Color.cyan;
        currentTileHighlight.layer = 2;



        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                if(!(x>40 && x<60 && z>40 && z <60))
                //if (x == 1 && z == 1)
                {
                    gridArray[x, z].Build(Instantiate(wall,
                        new Vector3(x * cellSize + transform.position.x + offsetX + cellSize / 2.0f,
                            0.0f + transform.position.y,
                            z * cellSize + transform.position.z + offsetZ + cellSize / 2.0f),
                        Quaternion.identity,this.transform),BuildingType.WALL);

                }
            }
        }
        
        
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                if (gridArray[x, z].BuildingHandler != null && gridArray[x, z].BuildingHandler.buildingType == BuildingType.WALL)
                {
                    Wall wallComponent = gridArray[x, z].BuildingHandler as Wall;
                    wallComponent.SetWall();
                }
            }
        }
    }

    private void Update()
    {
        //highlight the cell the mouse is over
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 point = hit.point - transform.position;
                int indexX = Mathf.FloorToInt(point.x - offsetX / cellSize);
                int indexY = Mathf.FloorToInt(point.z - offsetZ / cellSize);
                //print(indexX + " " + indexY);

                currentTile = GetTile(new Vector2Int(indexX, indexY));
            }
            else
            {
                currentTile = null;
                currentTileHighlight.transform.position = new Vector3(100, 100, 100);
            }


            if (currentTile != null)
            {
                currentTileHighlight.transform.position = new Vector3(currentTile.x + cellSize / 2.0f, hit.point.y + 0.01f,
                    currentTile.y + cellSize / 2.0f);
                currentTileHighlight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            if (Input.GetMouseButtonDown(0) && currentTile != null && buildingMode)
            {
                if (currentTile.Building == null)
                {
                    placeBuilding(Mathf.FloorToInt(currentTile.x / cellSize) + 50,
                        Mathf.FloorToInt(currentTile.y / cellSize) + 50, building);
                    StartCoroutine(changeTileHighlightClr(0.5f, Color.green));
                }
                else
                {
                    Debug.Log("There is already a building here");
                    StartCoroutine(changeTileHighlightClr(0.5f, Color.red));
                }

                buildingMode = false;
            }else if (currentTile != null && currentTile.BuildingHandler != null && currentTile.BuildingHandler.buildingType == BuildingType.WALL)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentTile.BuildingHandler.tile.Destroy();
                }
            }
        }
    }

    /*
    void OnApplicationQuit()
    {
        PrefabUtility.UnloadPrefabContents(building);
        PrefabUtility.UnloadPrefabContents(wall);
        PrefabUtility.UnloadPrefabContents(wallSide);
    }
    */
    public bool placeBuilding(int x, int y, GameObject building)
    {
        if (gridArray[x, y].Building == null)
        {
            gridArray[x, y].Build(Instantiate(
                building,
                new Vector3(x * cellSize + transform.position.x + offsetX + cellSize / 2.0f,
                    0.0f + transform.position.y, y * cellSize + transform.position.z + offsetZ + cellSize / 2.0f),
                Quaternion.identity,transform
            ));
            //gridArray[x, y].building.GetComponentInChildren<Animator>().Play("Base Layer.Construction", 0, 0.0f);

            //buildingAnimator.Play("Base Layer.Construction", 0, 0.0f);
            StartCoroutine(buildingConstructionCrtn(2.0f));
            Vector3 bldngSize = gridArray[x, y].Building.GetComponentsInChildren<MeshRenderer>()[1].bounds.size;

            gridArray[x, y].Building.transform.localScale = new Vector3(cellSize / bldngSize.x, cellSize / bldngSize.x,
                cellSize / bldngSize.x);
            return true;
        }

        return false;
    }

    public Tile GetTile(Vector2Int vector2Int)
    {
        if (vector2Int.x > 0 && vector2Int.x < width && vector2Int.y > 0 && vector2Int.y < height)
        {
            return gridArray[vector2Int.x, vector2Int.y];
        }

        return null;
    }

    public IEnumerator changeTileHighlightClr(float seconds, Color color)
    {
        currentTileHighlight.GetComponent<Renderer>().material.color = color;
        yield return new WaitForSeconds(seconds);
        currentTileHighlight.GetComponent<Renderer>().material.color = Color.cyan;
    }

    public IEnumerator buildingConstructionCrtn(float time)
    {
        for (int i = 0; i < 100; i++)
        {
            buildingAnimator.SetFloat("ConstructionProcantage", i / 100.0f);
            yield return new WaitForSeconds(time / 100.0f);
        }
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