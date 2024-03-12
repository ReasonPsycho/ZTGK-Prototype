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
        combatTarget = FindClosestEnemy(5.0f);
        if (hasTarget)
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
            combatTarget = FindClosestPlayerUnit(5.0f);
        }

        if (state == EnemyState.CHASE && distanceToTarget <= unit.reachRange)
        {
            state = EnemyState.ATTACK;
        }

        if (state == EnemyState.ATTACK)
        {
            combatTarget = FindClosestPlayerUnit(5.0f);
            if (distanceToTarget > unit.reachRange)
            {
                state = EnemyState.CHASE;
            }
            else
            {

                if (combatTarget != null && !isAttackOnCooldown && Vector2Int.Distance(combatTarget.GetComponent<Unit>().gridPosition, unit.gridPosition) <= unit.reachRange)
                {
                    StartCoroutine(attackCrt(combatTarget));
                }
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

    private GameObject FindClosestPlayerUnit(float range)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject enemy in enemies)
        {

            float diff = Vector3.Distance(enemy.transform.position, position);
            if (diff > range)
            {
                continue;
            }

            if (diff < distance)
            {
                closest = enemy;
                distance = diff;
            }

        }
        return closest;
    }









    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wakeUpRange);
    }
}
