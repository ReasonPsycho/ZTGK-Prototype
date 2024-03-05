using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class Building : MonoBehaviour {
    public BuildingType buildingType = BuildingType.ANY;
    public Tile tile; // Waaay easier just to hold a tile is on
    virtual public bool DestroyBuilding()
    {
        Destroy(gameObject);
        return true;
    }

    public List<BuildingAction> GetActions() {
        return BuildingHelper.BuildingActions[ buildingType ];
    }

}
