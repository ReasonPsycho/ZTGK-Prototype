using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    ASLEEP,
    CHASE,
    ATTACK,
    DEAD
}


public class EnemyAI : UnitAI
{
    public EnemyState state;
    public float wakeUpRange = 5.0f;

    private float distanceToTarget;

    private new void Start()
    {
        base.Start();
        state = EnemyState.ASLEEP;
    }

    private void Update()
    {
        if(hasTarget)
            Vector2Int.Distance(unit.gridPosition, target);

        if (CheckForPlayerUnit())
        {
            state = EnemyState.CHASE;
        }
        else
        {
            state = EnemyState.ASLEEP;
        }

        if (state == EnemyState.CHASE)
        {
            if (hasTarget)
            {
                path = FindPathToTarget(FindNearestVacantTile(movementTarget));
            }
        }


        if (path != null && path.Count > 0)
        {
            
        }
        if (state == EnemyState.CHASE)
        {
            MoveOnPath();
        }

        if(state == EnemyState.CHASE && distanceToTarget <= unit.reachRange)
        {
            state = EnemyState.ATTACK;
        }

        if(state == EnemyState.ATTACK)
        {
            if (distanceToTarget > unit.reachRange)
            {
                state = EnemyState.CHASE;
            }
            else
            {
                //Attack
                //print("BOOM BOOM ATTACK UNIT BOOM BOOM ");
            }
        }

        //if(hasTarget && path.Count == 0)
        //{
        //    hasTarget = false;
        //}
    }

    private bool CheckForPlayerUnit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wakeUpRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("PlayerUnit"))
            {
                movementTarget = unit.grid.WorldToGridPosition(hitCollider.transform.position);
                hasTarget = true;
                return true;
            }
        }

        return false;
    }








    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wakeUpRange);
    }
}
