using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Equipment : MonoBehaviour
{
    public GameObject ItemPrefab;
    public GameObject Item1;

    public GameObject Item2;

    public MyCursor myCursor;
    // Start is called before the first frame update
    void Awake()
    {
        Item1 = gameObject.transform.Find("Item 1").gameObject;
        Item2 = gameObject.transform.Find("Item 2").gameObject;
    }

    private void OnEnable()
    {
        Unit selectedUnit = (Unit)myCursor.ListOfSelected[0];
        if (selectedUnit.Item1 != null)
        {
            GameObject tmp = Instantiate(ItemPrefab, Item1.transform);
            tmp.name = selectedUnit.Item1.Name;
            tmp.GetComponent<Item>().description = selectedUnit.Item1.Description;
            tmp.gameObject.GetComponent<Image>().sprite  = selectedUnit.Item1.icon;
        }

        if (selectedUnit.Item2 != null)
        {
            GameObject tmp = Instantiate(ItemPrefab, Item2.transform);
            tmp.name = selectedUnit.Item2.Name;
            tmp.GetComponent<Item>().description = selectedUnit.Item2.Description;
            tmp.GetComponent<Image>().sprite = selectedUnit.Item2.icon;

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