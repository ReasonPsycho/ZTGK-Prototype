using Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MyCursor : MonoBehaviour
{
    [FormerlySerializedAs("myCursorMode")] [FormerlySerializedAs("cursorMode")] [SerializeField]
    private MY_CURSOR_MODE myMyCursorMode = MY_CURSOR_MODE.DEFAULT;

    public Tile currentTile;
    private GameObject currentTileHighlight;

    private Grid grid;
    private ConstructionManager constructionManager;

    private Unit selectedUnit;

    private ISelectable hovered;
    public ISelectable selected;


    public GameObject Inventory;
    public GameObject Equipment;

    public Texture2D unitCursor;
    public Texture2D buildingCursor;

    private void Start()
    {
        constructionManager = GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>();

        grid = constructionManager.grid;
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
                
                ISelectable selectable = hit.transform.GetComponentInParent<ISelectable>();

                if (selectable != null)
                {
                    if (selectable != selected)
                    {
                        selectable.OnHoverEnter();
                    }

                    if (selectable != hovered)
                    {
                        if (hovered != null)
                        {
                            hovered.OnHoverExit();
                        }

                        hovered = selectable;
                    }

                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (selectable != selected)
                        {
                            if (selected != null)
                            {
                                selected.OnDeselect();
                            }

                            selected = selectable;
                            selectable.OnSelect();


                            switch (selectable.SelectionType)
                            {
                                case (SELECTION_TYPE.UNIT):
                                    myMyCursorMode = MY_CURSOR_MODE.UNIT;
                                    selectedUnit = (Unit)selectable;
                                    Cursor.SetCursor(unitCursor, Vector2.zero, CursorMode.Auto);
                                    break;

                                case (SELECTION_TYPE.BUILDING):
                                    myMyCursorMode = MY_CURSOR_MODE.BUILD;
                                    break;
                            }
                        }
                    }
                }
            }


            if (Input.GetMouseButtonDown(1) && selected != null && hovered != null)
            {
                if (selected != hovered)
                {
                    switch (myMyCursorMode)
                    {
                        case (MY_CURSOR_MODE.UNIT):
                            switch (selected.SelectionType)
                            {
                                case (SELECTION_TYPE.UNIT):
                                    if (((MonoBehaviour)(hovered)).gameObject.GetComponentInParent<Mineable>())
                                    {
                                        selectedUnit.GetComponent<UnitAI>().movementTarget =
                                            grid.WorldToGridPosition(hit.collider.transform.position);
                                        selectedUnit.GetComponent<UnitAI>().miningTarget =
                                            grid.WorldToGridPosition(hit.collider.transform.position);
                                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                        selectedUnit.GetComponent<UnitAI>().isGoingToMine = true;
                                    }


                                    else if (((MonoBehaviour)(hovered)).gameObject.GetComponentInParent<Unit>())
                                    {
                                        selectedUnit.GetComponent<UnitAI>().movementTarget =
                                            hit.collider.gameObject.GetComponentInParent<Unit>().gridPosition;
                                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                    }
                                    else
                                    {
                                        Vector3 target = hit.point;
                                        target.y = 0;
                                        selectedUnit.GetComponent<UnitAI>().movementTarget =
                                            grid.WorldToGridPosition(target);
                                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                    }

                                    break;

                                case (SELECTION_TYPE.BUILDING):
                                    //IDK
                                    break;
                            }

                            break;
                        case (MY_CURSOR_MODE.BUILD):
                            Building building = ((MonoBehaviour)(hovered)).GetComponent<Building>();
                            if (building != null)
                            {
                                currentTile = building.tile;
                                if (currentTile.Building == null ||
                                    currentTile.BuildingHandler.buildingType == BuildingType.FLOOR)
                                {
                                    constructionManager.placeBuilding(
                                        Mathf.FloorToInt(currentTile.x / grid.cellSize) + 50,
                                        Mathf.FloorToInt(currentTile.y / grid.cellSize) + 50,
                                        constructionManager.building);
                                }
                                else
                                {
                                    Debug.Log("There is already a building here");
                                }
                            }

                            break;
                    }
                }
            }
        }

        switch (myMyCursorMode)
        {
            case MY_CURSOR_MODE.UNIT:
                //      Cursor.SetCursor(unitCursor, Vector2.zero, CursorMode.Auto);
                Inventory.SetActive(true);
                Equipment.SetActive(true);
                break;
            case MY_CURSOR_MODE.BUILD:
                Cursor.SetCursor(buildingCursor, Vector2.zero, CursorMode.Auto);
                Inventory.SetActive(false);
                Equipment.SetActive(false);
                break;
            default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                Inventory.SetActive(false);
                Equipment.SetActive(false);
                break;
        }
        
        
    }


    public MY_CURSOR_MODE MyCursorMode
    {
        get => myMyCursorMode;
        set => myMyCursorMode = value;
    }
}