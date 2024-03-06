using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool isBuildingButton = true;

    public void OnClick()
    {
        if (isBuildingButton)
        {
            GameObject.Find("CursorAbstractObject").GetComponent<Cursor>().CursorMode = CursorMode.BUILD;
        }
        else
        {
            Debug.Log("Wall button clicked");
        }
    }
}
