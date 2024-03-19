using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using GameItems;
using GameItems.ConcreteItems;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class Chest : Building
{
    public float x;
    public float z;

    private Inventory inventory;

    private List<GameItem> items = new List<GameItem>();

    public override void Start()
    {
        base.Start();
        buildingType = BuildingType.CHEST;
        inventory = GameObject.Find("Item Managment").gameObject.GetComponent<ItemManager>().Inventory;
        items.Add(new GIBow());
        items.Add(new GIShield());
        items.Add(new GISword());
        //items.Add(new GIWashingPowder());
        //items.Add(new GILasso());
        //items.Add(new GIBucket());
    }

    public override void GetMaterial()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        orgColor = material.color;
    }
    
    public override bool DestroyBuilding()
    {
        inventory.AddItem(items[Random.Range(0, items.Count - 1)]);
        base.DestroyBuilding();
        return true;
    }
}