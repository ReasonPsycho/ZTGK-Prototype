using GameItems;
using UnityEngine;

public class UnitAI : MonoBehaviour, ISelectable
{
    public Unit unit;

    [Header("Mining")]
    public float miningSpeed = 1.0f;
    private float miningTime = 0;

    [Header("Combat")] 
    public UnitBehaviour Behaviour = UnitBehaviour.AUTO_ATTCK;
    public float attackSpeed = 1.0f;
    public float attackDamage = 10.0f;
    protected float attackCooldown = 0.0f;

    [Header("Target")]
    public Vector2Int target;
    public Vector2Int miningTarget;
    public GameObject combatTarget;


    [Header("ProjectilePrefabs")]
    public GameObject waterGunProjectilePrefab;

    [Header("Behavior flags")]
    public bool hasMiningTarget = false;
    [SerializeField] private bool isMining = false;


    private ConstructionManager constructionManager;

    #region ISelectable

    public SELECTION_TYPE SelectionType
    {
        get { return SELECTION_TYPE.UNIT; }
    }

    private Color orgColor;
    private Material material;
    private bool isHovered = false;

    #endregion



    protected  void Start()
    {
        constructionManager = GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>();
        unit = GetComponentInParent<Unit>();
        material = unit.material;
        orgColor = material.color;
    }

    private void Update()
    {
        HandleMining();

        HandleCombat();
    }

    #region ISelectable

    public void OnHoverEnter()
    {
        if (!isHovered)
        {
            unit.material.color = Color.blue;
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
            unit.material.color = Color.blue;
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

    #region Mining
    public void Mine(Vector2Int target)
    {

        if (Vector2Int.Distance(target, unit.gridPosition) <= 1.5f)
        {
            unit.hasReachedTarget = true;
            if (!isMining)
            {
                Mineable mineable;
                if (unit.grid.GetTile(target).Building.TryGetComponent<Mineable>(out mineable))
                {
                    unit.TurnTo(target);
                    miningTime = mineable.miningTime / miningSpeed;
                    isMining = true;
                }
                else
                {
                    hasMiningTarget = false;
                }
            }
        }
        else
        {
            unit.hasReachedTarget = false;
        }
    }

    #endregion

    #region Combat

    public void Attack(GameObject target)
    {
        unit.hasTarget = true;

       
        if (Vector2Int.Distance(target.GetComponentInParent<Unit>().gridPosition, unit.gridPosition) <= unit.reachRange * 1.5f)
        {
            unit.hasReachedTarget = true;
            unit.TurnTo(unit.grid.WorldToGridPosition(target.transform.position));
            unit.state = UnitState.ATTACKING;
            if (attackCooldown > 1.0f)
            {
                //RANGED ATTACK
                if (unit.Item1 != null || unit.Item2 != null)
                {
                    GameItem item;
                    if (unit.Item1 != null)
                    {
                        item = unit.Item1;
                    }
                    else
                    {
                        item = unit.Item2;
                    }
                    if (item.Name == "Water Gun")
                    {

                        RangedAttack(target, waterGunProjectilePrefab);
                    }
                }
                
                //MELEE ATTACK
                else
                {
                    target.GetComponentInParent<Unit>().TakeDmg(attackDamage + attackDamage * unit.PercentDamageBuff + unit.FlatDamageBuff, unit.Kockback, unit, unit.AOE);
                }
                    
                attackCooldown = 0.0f;
            }
            else
            {
                attackCooldown += Time.deltaTime;
            }
        }
        else
        {
            if (Behaviour == UnitBehaviour.AUTO_ATTCK && !unit.IsSelected)
            {
                unit.hasReachedTarget = false;
                unit.movementTarget = target.GetComponentInParent<Unit>().gridPosition;
            }
        }
    }


    public void RangedAttack(GameObject target, GameObject projectilePrefab)
    {
        if(projectilePrefab == null)
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

    public GameObject FindClosestEnemy(float range)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject enemy in enemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI.state != EnemyState.ASLEEP)
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
        }
        return closest;
    }

    #endregion
    

    #region Misc
    //all these functions are made only in order to make the code more readable and easier to understand
    //and are only to be used in the Update function

    private void HandleMining()
    {
        if (hasMiningTarget)
        {
            Mine(miningTarget);
        }
        if (isMining)
        {
            unit.state = UnitState.MINING;
            if (miningTime > 0)
            {
                miningTime -= Time.deltaTime;
            }
            else
            {
                constructionManager.destroyBuilding(unit.grid.GetTile(miningTarget).BuildingHandler);
                hasMiningTarget = false;
                isMining = false;
            }
        }

    }

    private void HandleCombat()
    {

        if (combatTarget == null)
        {
            combatTarget = FindClosestEnemy(5.0f);
        }
        else if (combatTarget != null || !unit.forceMove)
        {
            unit.movementTargetDistance = unit.reachRange;
            Attack(combatTarget);
        }
        else if(!(unit.state == UnitState.MINING && isMining))
        {
            unit.state = UnitState.IDLE;
        }
    }

    #endregion
}
