using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Building
{
    #region ISelectable

    private Color orgColor;
    private Material material;
    private bool isHovered = false;
    private bool isSelected = false;

    #endregion

    public void Start()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        orgColor = material.color;
    }

    #region ISelectable

    public override void OnHoverEnter()
    {
        if (!isHovered)
        {
            material.color = Color.cyan;
            isHovered = true;
        }
    }

    public override void OnHoverExit()
    {
        if (!isSelected)
        {
            material.color = orgColor;
        }

        isHovered = false;
    }

    public override void OnSelect()
    {
        if (!isSelected)
        {
            material.color = Color.blue;
        }

        isSelected = true;
        
    }

    public override void OnDeselect()
    {
        material.color = orgColor;
        isHovered = false;
        isSelected = false;
    }

    #endregion

    // Start is called before the first frame update
}