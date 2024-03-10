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

    private ISelectable selected;
    
    private void Start()
    {
        constructionManager = GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>();

        grid = constructionManager.grid;

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
                ISelectable selectable = hit.transform.GetComponentInParent<ISelectable>();
                
                if (selectable != null) {
                    selectable.OnHoverEnter();
                }

                if (selectable != selected)
                {
                    if (selected != null)
                    {
                        selected.OnHoverExit();
                    }
                    selected = selectable;
                }
            }
            
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
            }
            

            //BUILDING
            if (Input.GetMouseButtonDown(0) && currentTile != null && cursorMode == CursorMode.BUILD)
            {
                if (currentTile.Building == null)
                {

                    constructionManager.placeBuilding(Mathf.FloorToInt(currentTile.x / grid.cellSize) + 50,
                        Mathf.FloorToInt(currentTile.y / grid.cellSize) + 50, constructionManager.building);
                }
                else
                {
                    Debug.Log("There is already a building here");
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
                    //MINING
                    if (hit.collider.gameObject.GetComponentInParent<Mineable>())
                    {
                        selectedUnit.GetComponent<UnitAI>().movementTarget = grid.WorldToGridPosition(hit.collider.transform.position);
                        selectedUnit.GetComponent<UnitAI>().miningTarget = grid.WorldToGridPosition(hit.collider.transform.position);
                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                        selectedUnit.GetComponent<UnitAI>().isGoingToMine = true;
                    }


                    else if (hit.collider.gameObject.GetComponentInParent<Unit>())
                    {
                        selectedUnit.GetComponent<UnitAI>().movementTarget = hit.collider.gameObject.GetComponentInParent<Unit>().gridPosition;
                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                    }
                    else
                    {
                        Vector3 target = hit.point;
                        target.y = 0;
                        selectedUnit.GetComponent<UnitAI>().movementTarget = grid.WorldToGridPosition(target);
                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                    }
                }
            }


        }
    }


    public CursorMode CursorMode
    {
        get => cursorMode;
        set => cursorMode = value;
    }

}
