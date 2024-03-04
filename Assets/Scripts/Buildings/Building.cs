using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class Building : MonoBehaviour {
    public BuildingType buildingType = BuildingType.ANY;

    public bool DestroyBuilding()
    {
        return true;
    }

    public List<BuildingAction> GetActions() {
        return BuildingHelper.BuildingActions[ buildingType ];
    }

}
