using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using GameItems;
using GameItems.ConcreteItems;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Unit : MonoBehaviour
{
    public UnitState state = UnitState.IDLE;

    public UnitType type;

    public bool IsSelected = false;

    public Grid grid;
    public Animator animator;
    public Vector2Int gridPosition;
    public Tile currentTile;
    private Tile prevTile;
    public float facingAngle;

    [Header("PathFinding")]
    public ArrayList path;
    private Coroutine moving;
    public Vector2Int movementTarget;
    [SerializeField] private Vector2Int nextTile;
    [SerializeField] private int pathLength;
    private float t = 0;


    [Header("General")]
    public float MaxHealth = 100.0f;
    [SerializeField] private float health;
    public float reachRange = 1.0f;


    [Header("Movement")]
    public float tilesPerSecond = 3.0f;
    public float rotationSpeed = 3.0f;
    public bool isMoving = false;
    public bool hasTarget = false;


    [Header("Equipment")]
    public GameItem Item1;
    public GameItem Item2;
    public GameObject ItemGameObject1;
    public GameObject ItemGameObject2;


    private bool isFlashing = false;

    public float FlatDamageBuff = 0;        // Accumulators to avoid checking both items every attack.
    public float PercentDamageBuff = 0;     // Modified on Apply/Unapply, Equip/Unequip.
    public float Armor = 0;
    public float FlatAttackSpeedBuff = 0;
    public float FlatAttackRangeBuff = 0;
    public Material material;

    public void Awake()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
    }

    public void Start()
    {

        animator = transform.GetChild(0).GetComponent<Animator>();
        health = MaxHealth;

        ItemGameObject1 = transform.Find("Body/Item1").gameObject;
        ItemGameObject2 = transform.Find("Body/Item2").gameObject;
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        gridPosition = grid.WorldToGridPosition(transform.position);
        prevTile = grid.GetTile(gridPosition);
        path = new ArrayList();

    }


    private void Update()
    {
        UpdateCurrentTile();

        HandleMovement();

        UpdateAnimatorFlags();

        prevTile = currentTile;
    }


    #region Health
    public void TakeDmg(float dmg)
    {
        if (!isFlashing)
        {
            StartCoroutine(flashRedOnDmgTaken());
        }

        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator flashRedOnDmgTaken()
    {
        Color orgColor = material.color;
        material.color = Color.red;
        isFlashing = true;
        yield return new WaitForSeconds(0.1f);
        material.color = orgColor;
        isFlashing = false;
    }

    public void Heal(float hp)
    {
        health += hp;
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
    }

    private void Die()
    {
        currentTile.Vacant = true;
        Destroy(gameObject);
    }

    #endregion

    #region Equipment

    /// <summary>
    /// Try equip item to specified slot, any by default.
    /// Switch the items if force is on.
    /// Does not use Unequip.
    /// </summary>
    /// <param name="item">Item to equip</param>
    /// <param name="slot">Slot to fill. 1 for Item1, 2 for Item2.</param>
    /// <param name="force">If true, remove the previously equipped item. If true and slotMask is Any and both slots are filled, Item1 will be unequipped.</param>
    /// <returns>
    /// Tuple of boolean marking success or failure and GameItem that was unequipped in force mode, null otherwise.
    /// </returns>
    /// <returns></returns>
    public (bool, GameItem) Equip(GameItem item, int slot = 1, bool force = false)
    {
        if (item == null) return (false, null);

        if (slot == 1 && Item1 == null)
        {
            Item1 = item;
            Apply(Item1);
            ItemGameObject1.SetActive(true);
            return (true, null);
        }

        if (slot == 2 && Item2 == null)
        {
            Item2 = item;
            Apply(Item2);
            ItemGameObject2.SetActive(true);
            return (true, null);
        }

        if (force)
        {
            if (slot == 1)
            {
                var ret = Item1;
                Item1 = item;
                Unapply(ret);
                Apply(item);
                ItemGameObject2.SetActive(false);
                ItemGameObject1.SetActive(true);
                return (true, ret);
            }

            if (slot == 2)
            {
                var ret = Item2;
                Item2 = item;
                Unapply(ret);
                Apply(item);
                ItemGameObject1.SetActive(false);
                ItemGameObject2.SetActive(true);
                return (true, ret);
            }
        }

        return (false, null);
    }

    /// <summary>
    /// Unequips specified items and returns them as a list.
    /// </summary>
    /// <param name="slotMask">Slot to free. 1 for Item1, 2 for Item2, 3 for both.</param>
    /// <returns></returns>
    public List<GameItem> Unequip(int slotMask)
    {
        List<GameItem> ret = new();

        if ((slotMask & 0b01) != 0 && Item1 != null)
        {
            ret.Add(Item1);
            Unapply(Item1);
            ItemGameObject1.SetActive(false);
            Item1 = null;
        }

        if ((slotMask & 0b10) != 0 && Item2 != null)
        {
            ret.Add(Item2);
            Unapply(Item2);
            ItemGameObject2.SetActive(false);
            Item2 = null;
        }

        return ret;
    }

    /// <summary>
    /// Apply item buffs to accumulator stats.
    /// </summary>
    /// <param name="item">Item to apply.</param>
    public void Apply(GameItem item)
    {
        if (item == null) return;
        MaxHealth += item.HealthBuff;
        FlatDamageBuff += item.FlatDamageBuff;
        PercentDamageBuff += item.PercentDamageBuff;
        Armor += item.ArmorBuff;
        FlatAttackSpeedBuff += item.FlatAttackSpeedBuff;
        FlatAttackRangeBuff += item.FlatAttackRangeBuff;
    }

    /// <summary>
    /// Remove applied buffs from accumulator stats.
    /// </summary>
    /// <param name="item">Item to unapply.</param>
    public void Unapply(GameItem item)
    {
        if (item == null) return;

        MaxHealth -= item.HealthBuff;
        FlatDamageBuff -= item.FlatDamageBuff;
        PercentDamageBuff -= item.PercentDamageBuff;
        Armor -= item.ArmorBuff;
        FlatAttackSpeedBuff -= item.FlatAttackSpeedBuff;
        FlatAttackRangeBuff -= item.FlatAttackRangeBuff;
    }

    #endregion

    #region Movement
    public void MoveOnPath()

    {

        if (isMoving) return;
        isMoving = true;


        nextTile = (Vector2Int)path[0];
        if (path.Count > 1)
        {
            nextTile = (Vector2Int)path[1];
        }
        else if (path.Count == 1)
        {
            isMoving = false;
            path.Clear();
            return;
        }

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(grid.GridToWorldPosition(nextTile).x, grid.GridToWorldPosition(nextTile).z)) < 1.5f)
        {
            t = 0;
            path.RemoveAt(0);
        }
    }

    public void StopMoving()
    {
        path.Clear();
        isMoving = false;
        state = UnitState.IDLE;
        hasTarget = false;
    }


    public ArrayList FindPathToTarget(Vector2Int target)
    {
        ArrayList result = new ArrayList();

        // Get the current unit position
        Vector2Int startPosition = grid.WorldToGridPosition(transform.position);

        // Perform A* pathfinding
        List<Vector2Int> waypoints = AStarSearch(startPosition, target);

        // If a path is found, store it in the path ArrayList
        if (waypoints != null)
        {
            foreach (Vector2Int waypoint in waypoints)
            {
                result.Add(waypoint);
            }
        }
        return result;
    }

    public void TurnTo(Vector2Int target)
    {
        Vector3 targetPos = grid.GridToWorldPosition(target);
        targetPos.y = transform.position.y;
        Vector3 dir = targetPos - transform.position;
        dir.y = 0; // Keep the direction in the XZ plane
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
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
            if (grid.GetTile(current).Vacant || current == grid.WorldToGridPosition(transform.position))
            {
                return current;
            }
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (grid.GetTile(next) != null && !visited.Contains(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }
        return target;

    }


    #region A* Pathfinding
    private List<Vector2Int> AStarSearch(Vector2Int start, Vector2Int target)
    {
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Vector2Int.Distance(start, target);

        while (openSet.Count > 0)
        {
            Vector2Int current = GetLowestFScoreNode(openSet, fScore);
            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current] + Vector2Int.Distance(current, neighbor);
                if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector2Int.Distance(neighbor, target);
                    if (!openSet.Contains(neighbor) && grid.GetTile(neighbor).Vacant)
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }

    #region A* Helper Functions
    private Vector2Int GetLowestFScoreNode(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        Vector2Int lowestNode = Vector2Int.zero;
        float lowestFScore = Mathf.Infinity;

        foreach (Vector2Int node in openSet)
        {
            if (fScore.ContainsKey(node) && fScore[node] < lowestFScore)
            {
                lowestNode = node;
                lowestFScore = fScore[node];
            }
        }

        return lowestNode;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        neighbors.Add(new Vector2Int(current.x + 1, current.y)); // Right
        neighbors.Add(new Vector2Int(current.x - 1, current.y)); // Left
        neighbors.Add(new Vector2Int(current.x, current.y + 1)); // Up
        neighbors.Add(new Vector2Int(current.x, current.y - 1)); // Down


        if (grid.GetTile(new Vector2Int(current.x + 1, current.y)).Vacant && grid.GetTile(new Vector2Int(current.x, current.y + 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x + 1, current.y + 1)); // Top Right

        if (grid.GetTile(new Vector2Int(current.x - 1, current.y)).Vacant && grid.GetTile(new Vector2Int(current.x, current.y + 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x - 1, current.y + 1)); // Top Left

        if (grid.GetTile(new Vector2Int(current.x + 1, current.y)).Vacant && grid.GetTile(new Vector2Int(current.x, current.y - 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x + 1, current.y - 1)); // Bottom Right

        if (grid.GetTile(new Vector2Int(current.x - 1, current.y)).Vacant && grid.GetTile(new Vector2Int(current.x, current.y - 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x - 1, current.y - 1)); // Bottom Left


        return neighbors;
    }
    #endregion
    #endregion
    #endregion

    #region Misc
    //all these functions are made only in order to make the code more readable and easier to understand
    //and are only to be used in the Update function

    private void UpdateAnimatorFlags()
    {
        if (state == UnitState.MOVING)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsWorking", false);
        }
        else if (state == UnitState.MINING || state == UnitState.ATTACKING)
        {
            animator.SetBool("IsWorking", true);
            animator.SetBool("IsWalking", false);
        }
        else
        {
            animator.SetBool("IsWorking", false);
            animator.SetBool("IsWalking", false);
        }
    }

    private void UpdateCurrentTile()
    {
        gridPosition = grid.WorldToGridPosition(transform.position);
        currentTile = grid.GetTile(gridPosition);
        grid.GetTile(gridPosition).Vacant = false;

        if (prevTile != currentTile)
        {
            prevTile.Vacant = true;
        }
    }

    private void HandleMovement()
    {
        pathLength = path.Count;
        path.Clear();
        
        if (hasTarget)
        {
            path = FindPathToTarget(movementTarget);
            pathLength = path.Count;

        }

        if (hasTarget && path.Count < 1)
        {
            movementTarget = FindNearestVacantTile(movementTarget);
        }

        if (path.Count != 0)
        {
            MoveOnPath();
        }
        else
        {
            hasTarget = false;
            isMoving = false;
        }
        if (isMoving)
        {
            state = UnitState.MOVING;
            Vector3 targetPos = grid.GridToWorldPosition(nextTile);
            targetPos.y = transform.position.y;
            Vector3 startPos = transform.position;
            startPos.y = transform.position.y;
            if (t <= 1.0f)
            {
                animator.SetFloat("motionTime", t);
                t += Time.deltaTime * tilesPerSecond;
                transform.position = Vector3.Lerp(startPos, targetPos, t + 0.01f);

                Vector3 dir = targetPos - transform.position;
                dir.y = 0; // Keep the direction in the XZ plane
                if (Vector3.Distance(Vector3.zero, dir) > 0.01f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), t);
                }
            }
            else
            {
                isMoving = false;
            }
        }

    }

    #endregion
}