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


public class EnemyAI : MonoBehaviour
{
    public EnemyState state;
    public float wakeUpRange = 5.0f;

    private float distanceToTarget;

    public Unit unit;




    [Header("Combat")]
    public float attackSpeed = 1.0f;
    public float attackDamage = 10.0f;
    protected bool isAttackOnCooldown = false;

    [Header("Target")]
    public Vector2Int target;
    public GameObject combatTarget;
    public float distanceToCombatTarget;

    protected void Start()
    {
        state = EnemyState.ASLEEP;
        unit = GetComponentInParent<Unit>();

    }

    private void Update()
    {
        distanceToCombatTarget = float.MaxValue;
        if (CheckForPlayerUnit())
        {
            combatTarget = FindClosestPlayerUnit(wakeUpRange * 1.5f);
            unit.hasTarget = true;
            if (combatTarget != null)
            {
                distanceToCombatTarget = Vector3.Distance(combatTarget.transform.position, transform.position);
            }
        }

        

        if (combatTarget != null && distanceToCombatTarget > unit.reachRange)
        {
            state = EnemyState.CHASE;
            unit.movementTarget = unit.FindNearestVacantTile(unit.grid.WorldToGridPosition(combatTarget.transform.position));
           
        }
        else if(combatTarget != null && distanceToCombatTarget <= unit.reachRange)
        {
            state = EnemyState.ATTACK;
            Attack(combatTarget);
        }
        
    }


    #region Lookin for player

    private bool CheckForPlayerUnit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wakeUpRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("PlayerUnit"))
            {
                unit.movementTarget = unit.grid.WorldToGridPosition(hitCollider.transform.position);
                unit.hasTarget = true;
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


    #endregion

    #region Combat

    public void Attack(GameObject target)
    {
        unit.TurnTo(unit.grid.WorldToGridPosition(target.transform.position));
        if (!isAttackOnCooldown)
        {
            target.GetComponentInParent<Unit>().TakeDmg(attackDamage + attackDamage * unit.PercentDamageBuff + unit.FlatDamageBuff);
            isAttackOnCooldown = true;
            StartCoroutine(AttackCooldown());
        }
    }

    public IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1.0f / attackSpeed);
        isAttackOnCooldown = false;
       
    }

    #endregion
}
