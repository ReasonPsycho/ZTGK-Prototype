using System;
using System.Collections;
using System.Collections.Generic;
using GameItems;
using GameItems.ConcreteItems;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameItem> GameItems = new();

    public GameObject gridObject;
    public ItemManager ItemManagment;

    private void Start()
    {  
        ItemManagment = GameObject.Find("Item Managment").GetComponent<ItemManager>();
        gridObject = transform.Find("Inventory Grid").gameObject;
        GameItems.Add(new GIMop());
        GameItems.Add(new GIMop());
        GameItems.Add(new GILanceMop());
        GameItems.Add(new GILanceMop());
        GameItems.Add(new GITidePodLauncher());
        GameItems.Add(new GITidePodLauncher());

        foreach (var gameItem in GameItems)
        {
            GameObject tmp = Instantiate(ItemManagment.ItemPrefab, gridObject.transform);
            tmp.name = gameItem.Name;
            tmp.GetComponent<Item>().GameItem = gameItem;
            tmp.GetComponent<Image>().sprite = gameItem.icon;
        }
    }
}
