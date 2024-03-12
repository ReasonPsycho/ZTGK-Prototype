using System.Collections;
using System.Collections.Generic;
using GameItems;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    public Vector2 orginalPos;
    public ItemManager ItemManagment;
    public GameItem GameItem;
    public GameObject descriptionGameObject;
    public GameObject nameGameObject;


    void Start()
    {
        ItemManagment = GameObject.Find("Item Managment").GetComponent<ItemManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        orginalPos = gameObject.GetComponent<RectTransform>().position;
    }

    void addItemToEqiupmentSlot(GameObject eqiupmentSlot, int slot)
    {
        if (transform.parent == eqiupmentSlot.transform)
        {
            gameObject.GetComponent<RectTransform>().position = orginalPos;
        }
        else
        {
            Unit selectedUnit = ItemManagment.MyCursor.GetFirstSelectedUnit();
            if (selectedUnit != null)
            {
                if (eqiupmentSlot.transform.childCount > 0)
                {
                    GameItem tmpItem = selectedUnit.Unequip(slot)[0];
                    eqiupmentSlot.transform.GetChild(0).SetParent(transform.parent);
                    ItemManagment.Inventory.GameItems.Add(tmpItem);
                }

                ItemManagment.Inventory.GameItems.Remove(GameItem);
                selectedUnit.Equip(GameItem, slot);
                transform.SetParent(eqiupmentSlot.transform);
            }
        }
    }

    void switchEqiupmentSlot(GameObject eqiupmentSlotFrom,GameObject eqiupmentSlotTo, int fromSlot,
        int toSlot) //YEah prob easier way to do it exist idk
    {
        Unit selectedUnit = ItemManagment.MyCursor.GetFirstSelectedUnit();
        if (selectedUnit != null)
        {
            selectedUnit.Unequip(fromSlot);
            List<GameItem> tmpItem = selectedUnit.Unequip(toSlot);

            if (tmpItem.Count != 0)
            {
                eqiupmentSlotTo.transform.GetChild(0).SetParent(transform.parent);
                selectedUnit.Equip(GameItem, fromSlot);
            }

            transform.SetParent(eqiupmentSlotTo.transform);
            selectedUnit.Equip(GameItem, toSlot);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Unit selectedUnit = (Unit)ItemManagment.MyCursor.GetFirstSelectedUnit();
        if (RectTransformUtility.RectangleContainsScreenPoint(ItemManagment.inventoryGrid.GetComponent<RectTransform>(),
                Input.mousePosition))
        {
            if (transform.parent == ItemManagment.inventoryGrid.transform)
            {
                gameObject.GetComponent<RectTransform>().position = orginalPos;
            }
            else
            {
                if (GameItem == selectedUnit.Item1)
                {
                    selectedUnit.Unequip(1);
                }
                else
                {
                    selectedUnit.Unequip(2);
                }

                transform.SetParent(ItemManagment.inventoryGrid.transform);
            }
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(ItemManagment.Item1.GetComponent<RectTransform>(),
                     Input.mousePosition))
        {
            if (GameItem != selectedUnit.Item1)
            {
                if (GameItem != selectedUnit.Item2)
                {
                    addItemToEqiupmentSlot(ItemManagment.Item1, 1);
                }
                else
                {
                    switchEqiupmentSlot(ItemManagment.Item1,ItemManagment.Item2, 1, 2);
                }
            }
        }

        else if (RectTransformUtility.RectangleContainsScreenPoint(ItemManagment.Item2.GetComponent<RectTransform>(),
                     Input.mousePosition))
        {
            if (GameItem != selectedUnit.Item2)
            {
                if (GameItem != selectedUnit.Item1)
                {
                    addItemToEqiupmentSlot(ItemManagment.Item2, 2);
                }
                else
                {
                    switchEqiupmentSlot(ItemManagment.Item2,ItemManagment.Item1, 2, 1);
                }
            }
        }
        else
        {
            gameObject.GetComponent<RectTransform>().position = orginalPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionGameObject.GetComponent<TextMeshProUGUI>().text = GameItem.Description;
        nameGameObject.GetComponent<TextMeshProUGUI>().text = GameItem.Name;
        descriptionGameObject.SetActive(true);
        nameGameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionGameObject.SetActive(false);
        nameGameObject.SetActive(false);
    }
}