using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    private Unit unit;

    private void Start()
    { 
        
        unit = GetComponentInParent<Unit>();
        
    }

    private void Update()
    {
        
    }



    #region Movement

    public void MoveTo(Vector2Int target)
    {
        if (unit.state == UnitState.IDLE)
        {
            unit.state = UnitState.MOVING;
            StartCoroutine(MoveToCoroutine(target));
        }
    }

    private IEnumerator MoveToCoroutine(Vector2Int target)
    {
        Vector3 targetPos = unit.grid.GridToWorldPosition(target);
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0; // Keep the direction in the XZ plane
            transform.position += dir.normalized * unit.movementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), unit.rotationSpeed * Time.deltaTime);
            yield return null;
        }
        unit.state = UnitState.IDLE;
    }
    



    #endregion

    #region Mining

    #endregion

    #region Combat

    #endregion

    #region Equipment

    #endregion

}
