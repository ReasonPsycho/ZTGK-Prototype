using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public float x;
    public float y;
    public GameObject building;

    public Tile(float x, float y)
    {
        this.x = x;
        this.y = y;
        building = null;
    }
}
