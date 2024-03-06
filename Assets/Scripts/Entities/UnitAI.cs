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
        Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(target);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(target);
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (unit.grid.GetTile(current).Vacant)
            {
                return current;
            }
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (unit.grid.GetTile(next) != null && !visited.Contains(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }
        return target;
       
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
