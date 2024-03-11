using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class Building : MonoBehaviour, ISelectable
{
    public BuildingType buildingType = BuildingType.ANY;
    public List<Tile> tiles;
    public virtual Vector2Int Size { get; } = Vector2Int.one;


    virtual public bool DestroyBuilding()
    {
        Destroy(gameObject);
        foreach (var tile in tiles) {
            tile.Building = null; //TODO ensure this cleans memory
            tile.BuildingHandler = null;
        }
        return true;
    }


    #region ISelectable

    public SELECTION_TYPE SelectionType
    {
        get { return SELECTION_TYPE.BUILDING; }
    }

    public List<BuildingAction> GetActions()
    {
        return BuildingHelper.BuildingActions[buildingType];
    }

    public virtual void OnHoverEnter()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    public virtual void OnHoverExit()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    public virtual void OnSelect()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    public virtual void OnDeselect()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    #endregion
}