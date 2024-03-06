using Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private CursorMode cursorMode = CursorMode.DEFAULT;

    public Tile currentTile;
    private GameObject currentTileHighlight;

    private Grid grid;
    private ConstructionManager constructionManager;

    private UnitSelection unitSelection;
    private Unit selectedUnit;

    private void Start()
    {
        constructionManager = GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>();

        grid = constructionManager.grid;
        currentTileHighlight = GameObject.CreatePrimitive(PrimitiveType.Plane);
        currentTileHighlight.transform.position = new Vector3(100, 100, 100);
        currentTileHighlight.GetComponent<Renderer>().material.color = Color.cyan;
        currentTileHighlight.layer = 2;
        unitSelection = GameObject.Find("CursorAbstractObject").GetComponent<UnitSelection>();
    }

    private void Update()
    {
        selectedUnit = unitSelection.selectedUnit;
        
        //highlight the cell the mouse is over
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 point = hit.point - transform.position;
                int indexX = Mathf.FloorToInt(point.x - grid.offsetX / grid.cellSize);
                int indexY = Mathf.FloorToInt(point.z - grid.offsetZ / grid.cellSize);
                //print(indexX + " " + indexY);

                currentTile = grid.GetTile(new Vector2Int(indexX, indexY));
            }
            else
            {
                currentTile = null;
                currentTileHighlight.transform.position = new Vector3(100, 100, 100);
            }


            if (currentTile != null)
            {
                currentTileHighlight.transform.position = new Vector3(currentTile.x + grid.cellSize / 2.0f, hit.point.y + 0.01f,
                    currentTile.y + grid.cellSize / 2.0f);
                currentTileHighlight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            //BUILDING
            if (Input.GetMouseButtonDown(0) && currentTile != null && cursorMode == CursorMode.BUILD)
            {
                if (currentTile.Building == null)
                {

                    constructionManager.placeBuilding(Mathf.FloorToInt(currentTile.x / grid.cellSize) + 50,
                        Mathf.FloorToInt(currentTile.y / grid.cellSize) + 50, constructionManager.building);
                    StartCoroutine(changeTileHighlightClr(0.5f, Color.green));
                }
                else
                {
                    Debug.Log("There is already a building here");
                    StartCoroutine(changeTileHighlightClr(0.5f, Color.red));
                }

            }
            else if (currentTile != null && currentTile.BuildingHandler != null && currentTile.BuildingHandler.buildingType == BuildingType.WALL)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentTile.BuildingHandler.tile.Destroy();
                }
            }
            if (Input.GetMouseButtonDown(1) && cursorMode == CursorMode.BUILD)
            {
                cursorMode = CursorMode.DEFAULT;
            }
            //MOVING UNIT
            if(Input.GetMouseButtonDown(1) && currentTile != null && cursorMode == CursorMode.UNIT && selectedUnit != null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.GetComponentInParent<Unit>())
                    {
                        selectedUnit.GetComponent<UnitAI>().MoveUnit(hit.collider.gameObject.GetComponentInParent<Unit>().gridPosition);
                    }
                    else
                    {
                        Vector3 target = hit.point;
                        target.y = 0;
                        selectedUnit.GetComponent<UnitAI>().MoveUnit(selectedUnit.grid.WorldToGridPosition(target));
                    }
                }
            }

        }
    }

    public IEnumerator changeTileHighlightClr(float seconds, Color color)
    {
        currentTileHighlight.GetComponent<Renderer>().material.color = color;
        yield return new WaitForSeconds(seconds);
        currentTileHighlight.GetComponent<Renderer>().material.color = Color.cyan;
    }

    public CursorMode CursorMode
    {
        get => cursorMode;
        set => cursorMode = value;
    }

}
