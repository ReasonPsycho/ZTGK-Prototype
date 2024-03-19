using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool isBuildingButton = true;
    public GameObject buildingPrefab;
    public void OnClick()
    {
        if (isBuildingButton)
        {
            var cursor = GameObject.Find("CursorAbstractObject").GetComponent<MyCursor>();
                cursor.MyCursorMode = MY_CURSOR_MODE.BUILD;
                cursor.buildingPrefab = buildingPrefab;
                cursor.ListOfSelected.Select(selectable => selectable.SelectionType != SELECTION_TYPE.UNIT ? selectable : null).NotNull().ToList().ForEach(
                    selectable => {
                        selectable.OnDeselect();
                        cursor.ListOfSelected.Remove(selectable);
                    });
            GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>().building = buildingPrefab;
        }
        else
        {
            Debug.Log("Wall button clicked");
        }
    }
}
