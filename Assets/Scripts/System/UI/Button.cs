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
            GameObject.FindWithTag("Grid").GetComponent<Grid>().buildingMode = true;
        }
        else
        {
            Debug.Log("Wall button clicked");
        }
    }
}