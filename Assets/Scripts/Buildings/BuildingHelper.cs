using System.Collections.Generic;
using GameItems.ConcreteItems;
using UnityEngine;

namespace Buildings {
    public class BuildingHelper : MonoBehaviour {
        public static BuildingHelper Instance { get; set; } // for simpler access from across the project instead of adding as a field to every new spawned building, it's a manager anyway - do not make multiples of this class please
        public static Dictionary<BuildingType, List<BuildingAction>> BuildingActions = new();
        public List<GameObject> spawnedOptions = new(); // this could be cached per building and managed with SetActive, #prototyp

        [Header("Required")]
        public GameObject buttonPrefab;
        public GameObject menuGrid;

        [Header("Action-specific")]
        public Inventory inventory;
        public GameObject unitPrefab;

        private void Awake() {
            Instance = this;
        }

        /// <summary>
        /// Initializes the Action map, add new actions here.
        /// </summary>
        public void Start() {
            #region Adding Actions

            List<BuildingAction> any = new() {
                new BuildingAction(
                    "Destroy",
                    tile => tile.Destroy()
                )
            };

            List<BuildingAction> shop = new() {
                new BuildingAction(
                    "Spawn Unit",
                    tile => {
                        // check neighbors
                        Tile spawnTile = null;

                        for (int y = tile.Index.y - 1; y <= tile.Index.y + 1; y++) {
                            for (int x = tile.Index.x; x <= tile.Index.x + 1; x++) {
                                var neighbor = tile.grid.GetTile(new Vector2Int(x, y));
                                if ( neighbor.Vacant ) {
                                    spawnTile = neighbor;
                                    goto outer_brk;
                                }
                            }
                        }

                    outer_brk:
                        if ( spawnTile == null ) return false;
                        Instantiate(
                            unitPrefab,
                            new Vector3(spawnTile.x, 0.4f, spawnTile.y),
                            Quaternion.identity
                        ).GetComponent<Unit>().type = UnitType.ALLY;

                        return true;
                    }
                ),
            };

            BuildingActions[ BuildingType.ANY ] = any;
            BuildingActions[ BuildingType.SHOP ] = shop;

            #endregion
        }

        /// <summary>
        /// Instantiates action menu buttons for the requested type.
        /// <remarks>Read BuildingActionExec for why this doesn't just assign a lambda to the button's click event.</remarks>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="buildingType"></param>
        public void AddOptions(Building sender, BuildingType buildingType) {
            // add options for type
            if ( BuildingActions.ContainsKey(buildingType) )
                foreach (var action in BuildingActions[ buildingType ]) {
                    var obj = Instantiate(buttonPrefab, menuGrid.transform);
                    var exec = obj.GetComponent<BuildingActionExec>();
                    exec.targetBuilding = sender;
                    exec.action = action;
                    spawnedOptions.Add(obj);
                }

            // add universal options
            if ( buildingType != BuildingType.ANY ) {
                foreach (var action in BuildingActions[ BuildingType.ANY ]) {
                    var obj = Instantiate(buttonPrefab, menuGrid.transform);
                    var exec = obj.GetComponent<BuildingActionExec>();
                    exec.targetBuilding = sender;
                    exec.action = action;
                    spawnedOptions.Add(obj);
                }
            }
        }

        /// <summary>
        /// Removes all added options.
        /// No reason to make this search per type (currently) - the options should be cleaned on each deselect/selection change. Unless for the spawnedOptions cache optimization.
        /// #prototyp
        /// </summary>
        public void RemoveOptions() {
            foreach (var obj in spawnedOptions) {
                Destroy(obj);
            }
        }
    }
}