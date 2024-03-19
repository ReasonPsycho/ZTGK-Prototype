using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class Building : MonoBehaviour, ISelectable
{
    #region ISelectable

    protected Color orgColor;
    public Material material;
    protected bool isHovered = false;
    protected bool isSelected = false;

    #endregion

    public BuildingType buildingType = BuildingType.ANY;
    public List<Tile> tiles = new();
    public virtual Vector2Int Size { get; } = new(7, 4);


    public virtual void Start() {
        GetMaterial();
   }

    virtual public void GetMaterial()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        orgColor = material.color;
    }

    virtual public bool DestroyBuilding()
    {
        BuildingHelper.Instance.RemoveOptions();
        Destroy(gameObject);
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

        BuildingHelper.Instance.AddOptions(this, buildingType);
        isSelected = true;
    }

    public virtual void OnDeselect()
    {
        material.color = orgColor;
        BuildingHelper.Instance.RemoveOptions();
        isHovered = false;
        isSelected = false;

    }

    #endregion
}