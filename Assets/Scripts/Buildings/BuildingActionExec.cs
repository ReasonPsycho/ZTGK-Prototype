using UnityEngine;

namespace Buildings {
    /// <summary>
    /// Slap this bad boy on an Action Button prefab and set the Button's OnClick to Exec();
    /// When Instantiating an Action Button set this script's fields.
    ///
    /// Exists because Button.clicked requires an Action with no return which doesn't allow us to store the action result. (You could just make a lambda discarding the result)
    /// Also remembers the sender building to create per-building objects, otherwise a good option for shared execution would be to pass the selected building from the cursor.
    /// </summary>
    public class BuildingActionExec : MonoBehaviour {
        public Building targetBuilding;
        public BuildingAction action;
        public bool lastSuccess = false;

        public void Exec() {
            lastSuccess = action.Function(targetBuilding.tiles[ 0 ]);
        }
    }
}