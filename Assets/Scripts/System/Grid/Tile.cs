using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public float x;
    public float y;
    public Vector2Int index;
    public bool vacant = true;

    public GameObject building = null;
    public Building buildingHandler = null;

    public bool Build(GameObject building) {
        if ( !vacant ) return false;
        vacant = false;
        this.building = building;
        return true;
    }

    public bool Destroy() {
        if ( vacant ) return false;
        vacant = true;
        this.building = null;
        return true;
    }

    public Tile(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
