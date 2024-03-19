using System.Collections;
using UnityEngine;

public enum EnemyState
{
    ASLEEP,
    CHASE,
    ATTACK,
    DEAD
}


public class EnemyAI : MonoBehaviour , ISelectable
{
    public EnemyState state;
    public float wakeUpRange = 5.0f;
    public bool isAwake = false;
    [Unity.Collections.ReadOnly] public Unit unit;

    [Header("Combat")]
    public float attackSpeed = 1.0f;
    public float attackDamage = 10.0f;
    private bool isAttackOnCooldown = false;

    [Header("Target")]
    public Vector2Int target;
    public GameObject combatTarget;
    public float distanceToCombatTarget;

    public float attackCooldown = 0;

    [Header("Projectile Prefab")]
    public GameObject dirtGunProjectilePrefab;

    #region ISelectable

    public SELECTION_TYPE SelectionType
    {
        get { return SELECTION_TYPE.UNIT; }
    }

    private Color orgColor;
    private Material material;
    private bool isHovered = false;

    #endregion

    
    private void Start()
    {
        state = EnemyState.ASLEEP;
        unit = GetComponentInParent<Unit>();
        material = unit.material;
        orgColor = material.color;
    }

    private void Update()
    {
        HandleTargetSelection();
        
        HandleCombat();
        
        if (!unit.IsSelected && !isHovered)
        {
            UpdateColor();
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
                unit.movementTarget =  hitCollider.GetComponentInParent<Unit>().gridPosition;
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

            float diff = Vector2Int.Distance(enemy.GetComponentInParent<Unit>().gridPosition, unit.gridPosition);
            
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
        unit.state = UnitState.ATTACKING;
        unit.TurnTo(unit.grid.WorldToGridPosition(target.transform.position));
        if (attackCooldown > 1.0f)
        {
            if (unit.reachRange < 5)
            {
                target.GetComponentInParent<Unit>().TakeDmg(attackDamage + attackDamage * unit.PercentDamageBuff + unit.FlatDamageBuff, 0, unit, 0);
            }
            else
            {
                RangedAttack(target, dirtGunProjectilePrefab);
            }
            attackCooldown = 0.0f;
        }
        else
        {
            attackCooldown += Time.deltaTime;
        }
    }


    public void RangedAttack(GameObject target, GameObject projectilePrefab)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is null");
            return;
        }
        //instantiate projectile in front of the unit
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 0.7f, Quaternion.identity);

        if (projectile != null)
        {
            Projectile pr = projectile.GetComponent<Projectile>();

            pr.target = target;
            pr.dmg = attackDamage + attackDamage * unit.PercentDamageBuff + unit.FlatDamageBuff;
            pr.dealer = unit;
            pr.aoe = unit.AOE;
            pr.knockback = unit.Kockback;
        }

        return;
    }

    #endregion


    #region Misc
    //all these functions are made only in order to make the code more readable and easier to understand
    //and are only to be used in the Update function

    private void HandleTargetSelection()
    {
        distanceToCombatTarget = float.MaxValue;
        if (CheckForPlayerUnit())
        {
            if (!unit.hasTarget)
            {
                combatTarget = FindClosestPlayerUnit(wakeUpRange * 1.5f);
            }
        }
        
        if (combatTarget != null)
        {
            distanceToCombatTarget = Vector2Int.Distance(combatTarget.GetComponentInParent<Unit>().gridPosition, unit.gridPosition);
            unit.hasTarget = true;
            unit.state = UnitState.MOVING;
        }
        else
        {
            unit.hasTarget = false;
            unit.state = UnitState.IDLE;
        }
        
    }

    private void HandleCombat()
    {
        if (combatTarget != null && distanceToCombatTarget > unit.reachRange)
        {
            state = EnemyState.CHASE;
            unit.state = UnitState.MOVING;
            unit.movementTarget = unit.FindNearestVacantTile(combatTarget.GetComponentInParent<Unit>().gridPosition,unit.gridPosition);
        }
        else if (combatTarget != null && distanceToCombatTarget <= unit.reachRange)
        {
            unit.path.Clear();
            state = EnemyState.ATTACK;
            unit.state = UnitState.ATTACKING;
            Attack(combatTarget);
        }

        if (combatTarget != null &&  unit.path.Count == 0)
        {
           unit.path.Clear();
        }
    }

    #endregion
    #region ISelectable

    private void UpdateColor()
    {
        material.color = Color.Lerp(Color.white, orgColor, unit.health / unit.MaxHealth);
    }
    
    public void OnHoverEnter()
    {
        if (!isHovered)
        {
            unit.material.color = Color.magenta;
            isHovered = true;
        }
    }

    public void OnHoverExit()
    {
        if (!unit.IsSelected)
        {
            unit.material.color = orgColor;
        }

        isHovered = false;
    }

    public void OnSelect()
    {
        if (!unit.IsSelected)
        {
            unit.material.color = Color.black;
        }
        unit.IsSelected = true;
    }

    public void OnDeselect()
    {
        unit.material.color = orgColor;
        isHovered = false;
        unit.IsSelected = false;
    }

    #endregion

}
