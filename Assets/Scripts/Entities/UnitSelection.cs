using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    //private ArrayList selectedUnits = new ArrayList();


    public Unit selectedUnit;
    private MyCursor _myCursor;


    private void Start()
    {
        _myCursor = GameObject.Find("CursorAbstractObject").GetComponent<MyCursor>();
    }

    private void Update()
    {
        /*
        //if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // when a unit is clicked
                if (hit.collider.gameObject.GetComponentInParent<Unit>())
                {
                    //if some unit is already selected
                    if (selectedUnit != null)
                    {
                        //if it is the same unit, disselect it
                        if (selectedUnit == hit.collider.gameObject.GetComponentInParent<Unit>())
                        {
                            selectedUnit.Deselect();
                            selectedUnit = null;
                            cursor.CursorMode = CursorMode.DEFAULT;
                        }
                        //if it is a different unit, disselect the previous one and select the new one
                        else
                        {
                            selectedUnit.Deselect();
                            selectedUnit = hit.collider.gameObject.GetComponentInParent<Unit>();
                            selectedUnit.Select();
                            cursor.CursorMode = CursorMode.UNIT;
                        }
                    }
                    //if no unit is selected, select the clicked unit
                    else
                    {
                        selectedUnit = hit.collider.gameObject.GetComponentInParent<Unit>();
                        selectedUnit.Select();
                        cursor.CursorMode = CursorMode.UNIT;
                    }

                }
                //if the clicked object is not a unit
                else
                {
                    if (selectedUnit != null)
                    {
                        selectedUnit.Deselect();
                        selectedUnit = null;
                        cursor.CursorMode = CursorMode.DEFAULT;
                    }
                }
            }
        }

*/
    }
}