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

        GameItems.ForEach(AddItem);
    }

    /// <summary>
    /// For adding items after Inventory initialization as well.
    /// Utility to set up the UI elements.
    /// </summary>
    /// <param name="gameItem"></param>
    public void AddItem(GameItem gameItem) {
        GameObject obj = Instantiate(ItemManagment.ItemPrefab, gridObject.transform);
        obj.name = gameItem.Name;
        obj.GetComponent<Item>().GameItem = gameItem;
        obj.GetComponent<Image>().sprite = gameItem.icon;
    }
}
