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
    public GameObject Behaviour;

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
                        List<Tile> highlightTiles = new();

                        #region Find build tiles
                        //v2
                        var dist = new Vector2(hitTile.x + 0.5f, hitTile.y + 0.5f) - new Vector2(hit.point.x, hit.point.z);
                        var right = dist.x >= 0;
                        var up = dist.y >= 0;

                        var sign_row = up ? -1 : 1;
                        var sign_col = right ? -1 : 1;
                        for (int y = 0; y < building.Size.y; y++) {
                            sign_row *= -1;
                            var tiley = hitTile.Index.y + sign_row * y;
                            if ( tiley < 0 || tiley >= grid.height ) {
                                sign_row *= -1;
                                tiley = hitTile.Index.y + sign_row * y;
                            }
                            for (int x = 0; x < building.Size.x; x++) {
                                sign_col *= -1;
                                var tilex = hitTile.Index.x + sign_col * x;
                                if ( tilex < 0 || tilex >= grid.width ) {
                                    sign_col *= -1;
                                    tilex = hitTile.Index.x + sign_col * x;
                                }
                                highlightTiles.Add( grid.GetTile( new Vector2Int( tilex, tiley ) ) );
                            }
                        }
                        #endregion

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
                                UnitAI selectedUnit = ((UnitAI)selected).GetComponent<UnitAI>();
                                   
                                if (((MonoBehaviour)(ListOfHovered[0])).gameObject.GetComponentInParent<EnemyAI>())
                                {
                                    selectedUnit.unit.movementTarget =
                                        hit.collider.gameObject.GetComponentInParent<Unit>().gridPosition;  
                                    selectedUnit.unit.movementTargetDistance =
                                        hit.collider.gameObject.GetComponentInParent<Unit>().reachRange;
                                    selectedUnit.unit.hasTarget = true;
                                    selectedUnit.unit.hasReachedTarget = false;
                                    selectedUnit.GetComponent<UnitAI>().combatTarget = hit.collider.gameObject;
                                    
                                }
                                else if (((MonoBehaviour)(ListOfHovered[0])).gameObject
                                         .GetComponentInParent<Mineable>())
                                {
                                    Vector3 target = hit.point;
                                    selectedUnit.unit.movementTarget =
                                        selectedUnit.unit.FindNearestVacantTile(selectedUnit.unit.grid.WorldToGridPosition(target),selectedUnit.unit.gridPosition);
                                    selectedUnit.unit.movementTargetDistance =
                                      0.0f;
                                    selectedUnit.GetComponent<UnitAI>().miningTarget =
                                        ((Building)ListOfHovered[0]).tiles[0].Index;
                                    selectedUnit.unit.hasTarget = true;
                                    selectedUnit.unit.forceMove = true;
                                    selectedUnit.unit.hasReachedTarget = false;
                                    selectedUnit.GetComponent<UnitAI>().hasMiningTarget = true;
                                }
                                else
                                {
                                    Vector3 target = hit.point;
                                    target.y = 0;
                                    selectedUnit.unit.movementTarget =
                                        grid.WorldToGridPosition(target);
                                    selectedUnit.unit.movementTargetDistance = 0.0f;
                                    selectedUnit.unit.hasTarget = true;
                                    selectedUnit.unit.forceMove = true;
                                    selectedUnit.unit.hasReachedTarget = false;
                                    selectedUnit.GetComponent<UnitAI>().combatTarget = null;
                                    selectedUnit.GetComponent<UnitAI>().hasMiningTarget = false;
                                    selectedUnit.GetComponent<Unit>().forceMove = true;
                                    
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
                Behaviour.SetActive(true);
                break;
            case MY_CURSOR_MODE.BUILD:
                Cursor.SetCursor(buildingCursor, Vector2.zero, CursorMode.Auto);
                Inventory.SetActive(false);
                Equipment.SetActive(false);
                Behaviour.SetActive(false);
                break;
            default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                Inventory.SetActive(false);
                Equipment.SetActive(false);
                Behaviour.SetActive(false);
                break;
        }
    }

    public  UnitAI GetFirstSelectedUnit()
    {
        if (ListOfSelected.Count != 0 && ListOfSelected[0].SelectionType == SELECTION_TYPE.UNIT)
        {
            UnitAI firstSelected;
            if (((MonoBehaviour)ListOfSelected[0]).TryGetComponent(out firstSelected))
            {
                return firstSelected;
            }
            else
            {
                return null;
            }
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