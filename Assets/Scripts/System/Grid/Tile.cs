using System.Collections;
using System.Collections.Generic;
using Buildings;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Tile {
    public float x;
    public float y;
    public Vector2Int Index;
    public bool Vacant = true;

    public GameObject Building;
    public Building BuildingHandler;
    public Grid grid;
    
    public bool Build(GameObject building) {
        if ( !Vacant ) return false;
        this.Building = building;
        BuildingHandler = building.GetComponent<Building>();
        BuildingHandler.tiles.Add(this);
        if(  BuildingHandler.buildingType != BuildingType.FLOOR)
        {
            Vacant = false;
        }
        return true;
    }
    public bool Build(GameObject building, BuildingType buildingType) {
        if ( !Vacant ) return false;
        this.Building = building;
        BuildingHandler = building.GetComponent<Building>();
        BuildingHandler.tiles.Add(this);
        BuildingHandler.buildingType = buildingType; 
        if( buildingType != BuildingType.FLOOR)
        {
            Vacant = false;
        }
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

    public Tile(float x, float y,Grid grid) {
        this.x = x;
        this.y = y;
        this.grid = grid;
    }
}