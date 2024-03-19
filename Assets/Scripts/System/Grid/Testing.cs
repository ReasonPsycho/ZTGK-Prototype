using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    public Grid Grid;
    int i = 0;
    private void Start()
    {
        Grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    private void Update()
    {
        //foreach (Tile tile in Grid.gridArray)
        //{
        //    if (tile.Building != null)
        //    {
        //        i++;
        //        Debug.Log("Building: " + tile.Building + " " + i);
        //    }
        //}
    }
}

