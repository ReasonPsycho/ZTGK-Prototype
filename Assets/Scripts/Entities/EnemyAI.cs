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



    private new void Start()
    {
        base.Start();
        state = EnemyState.ASLEEP;
    }

    private void Update()
    {
        if (state == EnemyState.ASLEEP)
        {
            if (CheckForPlayerUnit())
            {
                state = EnemyState.CHASE;
            }
        }
        else if (state == EnemyState.CHASE)
        {
            if (hasTarget)
            {
                path = FindPathToTarget(movementTarget);
            }
            else
            {
                state = EnemyState.ASLEEP;
            }
        }


        if (path != null && path.Count > 0)
        {
            if (state == EnemyState.CHASE)
            {
                MoveOnPath();
            }
        }
    }

    private bool CheckForPlayerUnit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wakeUpRange);
        foreach (var hitCollider in hitColliders)
        {
            print(hitCollider.gameObject.tag);
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
