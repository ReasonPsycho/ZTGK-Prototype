using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    //private ArrayList selectedUnits = new ArrayList();


    private Unit selectedUnit;

    private void Update()
    {
        if (selectedUnit == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.GetComponentInParent<Unit>())
                    {
                        if (selectedUnit != null)
                        {
                            selectedUnit.Deselect();
                        }
                        selectedUnit = hit.collider.gameObject.GetComponentInParent<Unit>();
                        selectedUnit.Select();
                    }
                    else
                    {
                        if (selectedUnit != null)
                        {
                            selectedUnit.Deselect();
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.GetComponentInParent<Unit>())   
                    {
                        selectedUnit.GetComponent<UnitAI>().MoveTo(hit.collider.gameObject.GetComponentInParent<Unit>().gridPosition);
                    }
                    else
                    {
                        Vector3 target = hit.point;
                        target.y = 0;
                        selectedUnit.GetComponent<UnitAI>().MoveTo(selectedUnit.grid.WorldToGridPosition(target));
                    }
                }
            }
        }

    }

}
