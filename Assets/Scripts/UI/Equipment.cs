using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public GameObject Item1;
    public GameObject Item2;
    public ItemManager ItemManagment;
    private Unit selectedUnit;

    public MyCursor myCursor;

    // Start is called before the first frame update
    void Awake()
    {
        ItemManagment = GameObject.Find("Item Managment").GetComponent<ItemManager>();
        Item1 = gameObject.transform.Find("Item 1").gameObject;
        Item2 = gameObject.transform.Find("Item 2").gameObject;
    }

    public void Update()
    {
        if (selectedUnit != null)
        {
            Unit tmpUnit = myCursor.GetFirstSelectedUnit();
            if (tmpUnit != null && tmpUnit != selectedUnit)
            {
                OnDisable();
                OnEnable();
            }
        }
    }

    private void OnEnable()
    {
        selectedUnit = (Unit)myCursor.GetFirstSelectedUnit();
        if (selectedUnit != null)
        {
            if (selectedUnit.Item1 != null)
            {
                GameObject tmp = Instantiate(ItemManagment.ItemPrefab, Item1.transform);
                tmp.name = selectedUnit.Item1.Name;
                tmp.GetComponent<Item>().GameItem = selectedUnit.Item1;
                tmp.GetComponent<Image>().sprite = selectedUnit.Item1.icon;
            }

            if (selectedUnit.Item2 != null)
            {
                GameObject tmp = Instantiate(ItemManagment.ItemPrefab, Item2.transform);
                tmp.name = selectedUnit.Item2.Name;
                tmp.GetComponent<Item>().GameItem = selectedUnit.Item2;
                tmp.GetComponent<Image>().sprite = selectedUnit.Item2.icon;

            }
        }
    }


    private void OnDisable()
    {
        foreach (Transform child in Item1.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in Item2.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}