using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Building
{
    private Color orgColor;
    private Material material;
    private bool hovered = false;
    public void Start()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
    }

    public override void OnHoverEnter()
    {
        if (!hovered)
        {
            orgColor = material.color;
            material.color = Color.cyan;
            hovered = true;
        }

    }
    
    public override void OnHoverExit()
    {
        material.color = orgColor;
        hovered = false;

    }
    // Start is called before the first frame update
}
