using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject inventoryGrid;
    public Inventory Inventory;
    public Equipment Equipment;
    public GameObject Item1;
    public GameObject Item2;
    public GameObject ItemPrefab;
    public MyCursor MyCursor;
    public bool isAutoAttacking = true;
    public UnityEngine.UI.Button Button;

    public void SetAutoAttack()
    {
        UnitAI unit = MyCursor.GetFirstSelectedUnit();

        if (unit != null)
        {
            if (unit.Behaviour == UnitBehaviour.AUTO_ATTCK)
            {
                unit.Behaviour = UnitBehaviour.STAY;
            }
            else
            {
                unit.Behaviour = UnitBehaviour.AUTO_ATTCK;
            }
        }
    }

    public void Update()
    {
        UnitAI unit = MyCursor.GetFirstSelectedUnit();
        if (unit != null)
        {
            Button.GetComponentInChildren<TextMeshProUGUI>().text = (unit.Behaviour == UnitBehaviour.AUTO_ATTCK ? "Unit is auto attacking" : "Unit staying");
        }
    }
}