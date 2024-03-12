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

    private Unit selectedUnit;

    private ISelectable hovered;
    public ISelectable selected;

    public List<ISelectable> hoveredList = new();

    // public List<Tile> buildList = new();
    public GameObject buildingPrefab;


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

                        foreach (var currentHover in hoveredList)
                        {
                            currentHover.OnHoverExit();
                        }

                        hoveredList.Clear();

                        foreach (var currentTile in highlightTiles)
                        {
                            currentTile.BuildingHandler.OnHoverEnter();
                            hoveredList.Add(currentTile.BuildingHandler);
                        }
                    }
                    else
                    {
                        if (hoveredList != null)
                        {
                            if (selectable != selected)
                            {
                                if (hoveredList.Count > 0 && selectable != hoveredList[0])
                                {
                                    hoveredList[0].OnHoverExit();
                                    hoveredList[0] = selectable;
                                }
                                else
                                {
                                    hoveredList.Add(selectable);
                                    hoveredList[0].OnHoverEnter();
                                }
                            }
                        }
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

                                    if (myMyCursorMode == MY_CURSOR_MODE.BUILD)
                                    {
                                        foreach (var currentHovered in hoveredList)
                                        {
                                            currentHovered.OnHoverExit();
                                        }

                                        hoveredList.Clear();
                                    }

                                    myMyCursorMode = MY_CURSOR_MODE.UNIT;


                                    selectedUnit = (Unit)selectable;
                                    Cursor.SetCursor(unitCursor, Vector2.zero, CursorMode.Auto);
                                    break;

                                case (SELECTION_TYPE.BUILDING):
                                    // myMyCursorMode = MY_CURSOR_MODE.BUILD;
                                    break;
                            }
                        }
                    }
                }
            }


            // if (Input.GetMouseButtonDown(1) && selected != null && hovered != null)
            if (Input.GetMouseButtonDown(1) && selected != null && hoveredList.Count != 0)
            {
                // if (selected != hovered)
                if (!hoveredList.Contains(selected))
                {
                    switch (myMyCursorMode)
                    {
                        case (MY_CURSOR_MODE.UNIT):
                            switch (selected.SelectionType)
                            {
                                case (SELECTION_TYPE.UNIT):
                                    if (((MonoBehaviour)(hoveredList[0])).gameObject.GetComponentInParent<Mineable>())
                                    {
                                        selectedUnit.GetComponent<UnitAI>().movementTarget =
                                            grid.WorldToGridPosition(hit.collider.transform.position);
                                        selectedUnit.GetComponent<UnitAI>().miningTarget =
                                            grid.WorldToGridPosition(hit.collider.transform.position);
                                        selectedUnit.GetComponent<UnitAI>().hasTarget = true;
                                        selectedUnit.GetComponent<UnitAI>().isGoingToMine = true;
                                    }
                                    else if (((MonoBehaviour)(hoveredList[0])).gameObject.GetComponentInParent<Unit>())
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
                                        selectedUnit.GetComponent<UnitAI>().combatTarget = null;
                                    }

                                    break;

                                case (SELECTION_TYPE.BUILDING):
                                    //IDK
                                    break;
                            }

                            break;
                        case (MY_CURSOR_MODE.BUILD):
                            if (buildingPrefab != null)
                            {
                                constructionManager.placeBuilding(
                                    grid, hoveredList.Select(selectable => ((Building)selectable).tiles[0]),
                                    buildingPrefab, BuildingType.SHOP
                                );
                            }

                            //    Building building = ((MonoBehaviour) (hoveredList[0])).GetComponent<Building>();
                            //        if ( building != null ) {
                            ////            currentTile = building.tiles[ 0 ];
                            //           if ( currentTile.Building == null ||
                            //                currentTile.BuildingHandler.buildingType == BuildingType.FLOOR ) {
                            //              constructionManager.placeBuilding(
                            //                 Mathf.FloorToInt(currentTile.x / grid.cellSize) + 50,
                            //                  Mathf.FloorToInt(currentTile.y / grid.cellSize) + 50,
                            //                 constructionManager.building);
                            //         } else {
                            //             Debug.Log("There is already a building here");
                            //         }
                            //       }

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