using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public float x;
    public float y;
    public Vector2Int Index;
    public bool Vacant = true;

    public GameObject Building;
    public Building BuildingHandler;

    public bool Build(GameObject building) {
        if ( !Vacant ) return false;
        Vacant = false;
        this.Building = building;
        BuildingHandler = building.GetComponent<Building>();
        return true;
    }

    public bool Destroy() {
        if ( Vacant ) return false;
        Vacant = true;
        BuildingHandler.DestroyBuilding();
        this.Building = null;
        this.BuildingHandler = null;
        return true;
    }

    public Tile(float x, float y) {
        this.x = x;
        this.y = y;
    }
}