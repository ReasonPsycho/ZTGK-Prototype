using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SELECTION_TYPE
{
    BUILDING,
    UNIT
};

public interface ISelectable
{
    SELECTION_TYPE SelectionType { get;}    
    void OnHoverEnter();

    void OnHoverExit();

    void OnSelect();

    void OnDeselect();
}
