using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 orginalPos;
    public GameObject inventoryGrid;
    public GameObject Item1;
    public GameObject Item2;
    public GameObject Details;
    private Vector2 startPos;

    void Start()
    {
        inventoryGrid = GameObject.Find("Inventory Grid");
        Item1 = GameObject.Find("Item 1");
        Item2 = GameObject.Find("Item 2");
        Details = GameObject.Find("Details");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        orginalPos = gameObject.GetComponent<RectTransform>().position;
    }
    
    void addItemToEqiupmentSlot(GameObject eqiupmentSlot)
    {
        if (transform.parent == eqiupmentSlot.transform)
        {
            gameObject.GetComponent<RectTransform>().position = orginalPos;
        }
        else
        {
            if ( eqiupmentSlot.transform.childCount > 0)
            {
                eqiupmentSlot.transform.GetChild(0).SetParent(transform.parent);
            }
            transform.SetParent(eqiupmentSlot.transform);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(inventoryGrid.GetComponent<RectTransform>(),
                Input.mousePosition))
        {
            if (transform.parent == inventoryGrid.transform)
            {
                gameObject.GetComponent<RectTransform>().position = orginalPos;
            }
            else
            {
                transform.SetParent(inventoryGrid.transform);
            }
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(Item1.GetComponent<RectTransform>(),
                     Input.mousePosition))
        {
            addItemToEqiupmentSlot(Item1);
        }
        
        else if (RectTransformUtility.RectangleContainsScreenPoint(Item2.GetComponent<RectTransform>(),
                     Input.mousePosition))
        {
            addItemToEqiupmentSlot(Item2);

        }
        else
        {
            gameObject.GetComponent<RectTransform>().position = orginalPos;
        }
    }

   

    bool isInsideZone(Vector3[] corners, BoxCollider2D collider2D)
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        PolygonCollider2D polygonCollider = this.GetComponent<PolygonCollider2D>();
        //Debug.Log("Sprite pos:" + rt.position.ToString());
        //Debug.Log("Border pos:" + corners[0].ToString() + corners[1].ToString());

        bool isIside = true;
        foreach (var corner in polygonCollider.GetPath(0))
        {
            if (!collider2D.bounds.Contains(corner + new Vector2(rt.position.x, rt.position.y)))
            {
                isIside = false;
            }
        }

        return isIside;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}