using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class Building : MonoBehaviour, ISelectable
{
    #region ISelectable

    private Color orgColor;
    protected Material material;
    private bool isHovered = false;
    private bool isSelected = false;

    #endregion

    public BuildingType buildingType = BuildingType.ANY;
    public List<Tile> tiles = new();
    public virtual Vector2Int Size { get; } = new(2, 2);

    private void Start() {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        orgColor = material.color;
    }

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
        if (!isHovered)
        {
            material.color = Color.cyan;
            isHovered = true;
        }

    }

    public virtual void OnHoverExit()
    {
        if (!isSelected)
        {
            material.color = orgColor;
        }

        isHovered = false;    }

    public virtual void OnSelect()
    {
        if (!isSelected)
        {
            material.color = Color.blue;
        }

        isSelected = true;    }

    public virtual void OnDeselect()
    {
        material.color = orgColor;
        isHovered = false;
        isSelected = false;

    }

    #endregion
}