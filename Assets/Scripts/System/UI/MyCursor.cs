using Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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


    public List<ISelectable> ListOfSelected = new();

    public List<ISelectable> ListOfHovered = new();

    // public List<Tile> buildList = new();
    public GameObject buildingPrefab;


    public GameObject Inventory;
    public GameObject Equipment;

    public Texture2D unitCursor;
    public Texture2D buildingCursor;

    private ISelectable startOfDrag;
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
                    if (myMyCursorMode == MY_CURSOR_MODE.BUILD)
                    {
                        var building = buildingPrefab.GetComponent<Building>();
                        var hitTile = grid.GetTile(grid.WorldToGridPosition(hit.point));
                        List<List<Tile>> sortedRows = new();
                        List<Tile> highlightTiles = new();


                        //todo check for grid edges or built tiles
                        for (int y = hitTile.Index.y - building.Size.y;
                             y < hitTile.Index.y + building.Size.y + 1;
                             y++)
                        {
                            List<Tile> sortedRow = new();

                            for (int x = hitTile.Index.x - building.Size.x;
                                 x < hitTile.Index.x + building.Size.x + 1;
                                 x++)
                            {
                                sortedRow.Add(grid.GetTile(new Vector2Int(x, y)));
                            }

                            sortedRow.Sort((tile, tile1) =>
                            {
                                var dist = Vector2.Distance(new Vector2(tile.x, tile.y),
                                    new Vector2(hit.point.x, hit.point.y));
                                var dist1 = Vector2.Distance(new Vector2(tile1.x, tile1.y),
                                    new Vector2(hit.point.x, hit.point.y));

                                return (int)(dist - dist1);
                            });
                            sortedRows.Add(sortedRow.GetRange(0, building.Size.x));
                        }

                        sortedRows.Sort((row, row1) =>
                        {
                            var dist = Vector2.Distance(new Vector2(row[0].x, row[0].y),
                                new Vector2(hit.point.x, hit.point.y));
                            var dist1 = Vector2.Distance(new Vector2(row1[0].x, row1[0].y),
                                new Vector2(hit.point.x, hit.point.y));

                            return (int)(dist - dist1);
                        });

                        for (int i = 0; i < building.Size.y; i++)
                        {
                            highlightTiles.AddRange(sortedRows[i]);
                        }

                        // hoveredList.AddRange(highlightTiles.Select(tile => tile.Building.GetComponent<ISelectable>()));

                        foreach (var currentHover in ListOfHovered)
                        {
                            currentHover.OnHoverExit();
                        }

                        ListOfHovered.Clear();

                        foreach (var currentTile in highlightTiles)
                        {
                            currentTile.BuildingHandler.OnHoverEnter();
                            ListOfHovered.Add(currentTile.BuildingHandler);
                        }
                    }
                    else
                    {
                        if (ListOfHovered != null)
                        {
                            if (!ListOfSelected.Contains(selectable))
                            {
                                if (ListOfHovered.Count > 0)
                                {
                                    if (selectable != ListOfHovered[0])
                                    {
                                        ListOfHovered[0].OnHoverExit();
                                    }

                                    ListOfHovered[0] = selectable;
                                    ListOfHovered[0].OnHoverEnter();
                                }
                                else
                                {
                                    ListOfHovered.Add(selectable);
                                    ListOfHovered[0].OnHoverEnter();
                                }
                            }
                        }
                    }

                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (ListOfSelected.Count != 0 && ListOfSelected[0] != null)
                        {
                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                foreach (var s in ListOfSelected)
                                {
                                    s.OnDeselect();
                                }

                                ListOfSelected.Clear();
                                ListOfSelected.Add(selectable);
                                selectable.OnSelect();
                            }
                            else
                            {
                                selectable.OnSelect();
                                ListOfSelected.Add(selectable);
                            }
                        }
                        else
                        {
                            selectable.OnSelect();
                            ListOfSelected.Add(selectable);
                        }

                        switch (selectable.SelectionType)
                        {
                            case (SELECTION_TYPE.UNIT):
                                // on exit from build mode clear its build-preview hover
                                if (myMyCursorMode == MY_CURSOR_MODE.BUILD)
                                {
                                    foreach (var currentHovered in ListOfHovered)
                                    {
                                        currentHovered.OnHoverExit();
                                    }

                                    ListOfHovered.Clear();
                                }

                                myMyCursorMode = MY_CURSOR_MODE.UNIT;
                                Cursor.SetCursor(unitCursor, Vector2.zero, CursorMode.Auto);
                                break;

                            // return to default cursor on background click
                            default:
                                // on exit from build mode clear its build-preview hover
                                if (myMyCursorMode == MY_CURSOR_MODE.BUILD)
                                {
                                    foreach (var currentHovered in ListOfHovered)
                                    {
                                        currentHovered.OnHoverExit();
                                    }

                                    ListOfHovered.Clear();
                                }
                                myMyCursorMode = MY_CURSOR_MODE.DEFAULT;

                                break;

                            // case (SELECTION_TYPE.BUILDING):
                            //     // myMyCursorMode = MY_CURSOR_MODE.BUILD;
                            //     break;
                        }
                    }
                }
            }


            // if (Input.GetMouseButtonDown(1) && selected != null && hovered != null)
            if (Input.GetMouseButtonDown(1) && ListOfSelected != null && ListOfHovered.Count != 0 &&
                ListOfSelected.Count != 0)
            {
                // if (selected != hovered)

                switch (myMyCursorMode)
                {
                    case (MY_CURSOR_MODE.UNIT):
                        foreach (var selected in ListOfSelected)
                        {
                            if (selected.SelectionType == SELECTION_TYPE.UNIT)
                            {
                                UnitAI selectedUnit = ((Unit)selected).GetComponent<UnitAI>();
                                    
                                if (((MonoBehaviour)(ListOfHovered[0])).gameObject.GetComponentInParent<Unit>())
                                {
                                    selectedUnit.GetComponent<UnitAI>().movementTarget =
                                        hit.collider.gameObject.GetComponentInParent<Unit>().gridPosition;
                                    selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                }
                                else if (((MonoBehaviour)(ListOfHovered[0])).gameObject
                                         .GetComponentInParent<Mineable>())
                                {
                                    Vector3 target = hit.point;
                                    selectedUnit.GetComponent<UnitAI>().movementTarget =
                                        grid.WorldToGridPosition(target);
                                    selectedUnit.GetComponent<UnitAI>().miningTarget =
                                        ((Wall)ListOfHovered[0]).tiles[0].Index;
                                    selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                    selectedUnit.GetComponent<UnitAI>().isGoingToMine = true;
                                }
                                else
                                {
                                    Vector3 target = hit.point;
                                    target.y = 0;
                                    selectedUnit.GetComponent<UnitAI>().movementTarget =
                                        grid.WorldToGridPosition(target);
                                    selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                    selectedUnit.GetComponent<UnitAI>().combatTarget = null;
                                }
                            }
                        }

                        break;
                    case (MY_CURSOR_MODE.BUILD):
                        if (buildingPrefab != null)
                        {
                            constructionManager.placeBuilding(
                                grid, ListOfHovered.Select(selectable => ((Building)selectable).tiles[0]),
                                buildingPrefab, BuildingType.SHOP
                            );
                        }
                        break;
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

    public  Unit GetFirstSelectedUnit()
    {
        if (ListOfSelected.Count != 0 && ListOfSelected[0].SelectionType == SELECTION_TYPE.UNIT)
        {
            return (Unit)ListOfSelected[0];
        }
        else
        {
            return null;
        }
    }
    
    public MY_CURSOR_MODE MyCursorMode
    {
        get => myMyCursorMode;
        set => myMyCursorMode = value;
    }
}