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

    public void TurnTo(Vector2Int target)
    {
        Vector3 targetPos = unit.grid.GridToWorldPosition(target);
        targetPos.y = transform.position.y;
        Vector3 dir = targetPos - transform.position;
        dir.y = 0; // Keep the direction in the XZ plane
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), unit.rotationSpeed * Time.deltaTime);
    }

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
        targetPos.y = transform.position.y;
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
    
    public bool MoveIfVacant(Vector2Int target)
    {
        if (unit.grid.GetTile(target).Vacant)
        {
            MoveTo(target);
            return true;
        }
        return false;
    }

   

    public Vector2Int FindNearestVacantTile(Vector2Int target)
    {
        Vector2Int nearest = target;
        float minDistance = float.MaxValue;
        for (int x = 0; x < unit.grid.width; x++)
        {
            for (int y = 0; y < unit.grid.height; y++)
            {
                if (unit.grid.GetTile(new Vector2Int(x, y)) == null) print(x + " " + y + " is null");
                if (unit.grid.GetTile(new Vector2Int(x, y)).Vacant)
                {
                    float distance = Vector2Int.Distance(target, new Vector2Int(x, y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = new Vector2Int(x, y);
                    }
                }
            }
        }
        print("Nearest vacant tile is " + nearest);
        return nearest;
    }

    //ONLY USE THIS FUNCTION TO MOVE THE UNIT
    public void MoveUnit(Vector2Int target)
    {
        if (!MoveIfVacant(target))
        {
            print("Target is not vacant, looking for new Tile");
            MoveIfVacant(FindNearestVacantTile(target));
        }
    }

    #endregion

    #region Mining

    #endregion

    #region Combat

    #endregion

    #region Equipment

    #endregion

}
