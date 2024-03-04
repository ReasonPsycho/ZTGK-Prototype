using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    private GameObject currentTileHighlight;

    GameObject building;
    Animator buildingAnimator;

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

        currentTileHighlight = GameObject.CreatePrimitive(PrimitiveType.Plane);
        currentTileHighlight.transform.position = new Vector3(100, 100, 100);
        currentTileHighlight.GetComponent<Renderer>().material.color = Color.cyan;

        building = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Building/Building.prefab") as GameObject;
        buildingAnimator = building.GetComponentInChildren<Animator>();

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
            //print(indexX + " " + indexY);


            currentTile = gridArray[indexX, indexY];
        }
        else
        {
            currentTile = null;
            currentTileHighlight.transform.position = new Vector3(100, 100, 100);
        }


        if (currentTile != null)
        {
                
            currentTileHighlight.transform.position = new Vector3(currentTile.x + cellSize/2.0f, 0.01f, currentTile.y + cellSize/2.0f);
            currentTileHighlight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

        if (Input.GetMouseButtonDown(0) && currentTile != null)
        {
            if (currentTile.building == null)
            {
                placeBuilding(Mathf.FloorToInt(currentTile.x / cellSize) + 50, Mathf.FloorToInt(currentTile.y / cellSize) + 50, building);
                StartCoroutine(changeTileHighlightClr(0.5f, Color.green));
            } else 
            {
                Debug.Log("There is already a building here");
                StartCoroutine(changeTileHighlightClr(0.5f, Color.red));
            }
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
            //gridArray[x, y].building.GetComponentInChildren<Animator>().Play("Base Layer.Construction", 0, 0.0f);

            //buildingAnimator.Play("Base Layer.Construction", 0, 0.0f);
            StartCoroutine(buildingConstructionCrtn(2.0f));
            return true;
        }
        return false;
    }

    
    public IEnumerator changeTileHighlightClr(float seconds, Color color){
        currentTileHighlight.GetComponent<Renderer>().material.color = color;
        yield return new WaitForSeconds(seconds);
        currentTileHighlight.GetComponent<Renderer>().material.color = Color.cyan;

    }

    public IEnumerator buildingConstructionCrtn(float time)
    {
        for (int i = 0; i < 100; i++)
        {
            buildingAnimator.SetFloat("ConstructionProcantage", i/100.0f);
            print(i);
            yield return new WaitForSeconds(time/100.0f);
        }
    }

}
