using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    protected Unit unit;


    [Header("PathFinding")]
    protected ArrayList path;
    private Coroutine moving;
    public Vector2Int target;
    public Vector2Int movementTarget;
    public Vector2Int miningTarget;
    public GameObject combatTarget;
    public bool hasTarget = false;
    
    private Vector2Int nextTile;
    private bool isMoving = false;

    private float t = 0;

    public bool isGoingToMine = false;
    private bool isMining = false;

    [SerializeField] private int pathLength;

    protected bool isAttackOnCooldown = false;
    protected void Start()
    {

        unit = GetComponentInParent<Unit>();
        path = new ArrayList();
    }

    private void Update()
    {
        pathLength = path.Count;
        if (hasTarget)
        {
            path.Clear();
            path = FindPathToTarget(movementTarget);
        }

        if (hasTarget && path.Count < 1)
        {
            movementTarget = FindNearestVacantTile(movementTarget);
        }

        if (path.Count != 0)
        {
            unit.state = UnitState.MOVING;
            MoveOnPath();
        }


        if (hasTarget && Vector3.Distance(transform.position, unit.grid.GridToWorldPosition(movementTarget)) < 0.05f)
        {
            hasTarget = false;
            isMoving = false;
            unit.state = UnitState.IDLE;
        }

        if (isGoingToMine)
        {
            Mine(miningTarget);

        }


        if (unit.type == UnitType.ALLY)
        {
            if (!unit.IsSelected)
            {
                combatTarget = FindClosestEnemy(5.0f);
            }
            if (combatTarget != null)
            {
                Attack(combatTarget);
            }
            else
            {
                hasTarget = false;
                unit.state = UnitState.IDLE;
            }
        }

        if (isMoving)
        {
            Vector3 targetPos = unit.grid.GridToWorldPosition(nextTile);
            targetPos.y = transform.position.y;
            Vector3 startPos = unit.grid.GridToWorldPosition((Vector2Int)path[0]);
            startPos.y = transform.position.y;
            if(t <= 1)
            {
                unit.animator.SetFloat("motionTime", t);
                t += Time.deltaTime * unit.tilesPerSecond;
                transform.position = Vector3.Lerp(startPos, targetPos, t + 0.01f);

                Vector3 dir = targetPos - transform.position;
                dir.y = 0; // Keep the direction in the XZ plane
                if (Vector3.Distance(Vector3.zero, dir) > 0.01f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), t * 5);
                }
                //transform.rotation = Quaternion.Slerp(transform.rotation, tra, unit.rotationSpeed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
            }
        }
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
            if (unit.grid.GetTile(current).Vacant || current == unit.grid.WorldToGridPosition(transform.position))
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

    public void MoveOnPath()
    {
        
        if (isMoving) return;
        isMoving = true;

        nextTile = (Vector2Int)path[0];
        if (path.Count > 1)
            nextTile = (Vector2Int)path[1];
        else
            isMoving = false;
        print(transform.position);
        print(unit.grid.GridToWorldPosition(nextTile));
        if (Vector2.Distance(new Vector2( transform.position.x,transform.position.z), new Vector2(unit.grid.GridToWorldPosition(nextTile).x,unit.grid.GridToWorldPosition(nextTile).z)) < 0.05f)
        {;
            print("Yea");
            t = 0;
            path.RemoveAt(0);
        }
    }


    public ArrayList FindPathToTarget(Vector2Int target)
    {
        ArrayList result = new ArrayList();

        // Get the current unit position
        Vector2Int startPosition = unit.grid.WorldToGridPosition(transform.position);

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
                    if (!openSet.Contains(neighbor) && unit.grid.GetTile(neighbor).Vacant)
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


        if (unit.grid.GetTile(new Vector2Int(current.x + 1, current.y)).Vacant && unit.grid.GetTile(new Vector2Int(current.x, current.y + 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x + 1, current.y + 1)); // Top Right

        if (unit.grid.GetTile(new Vector2Int(current.x - 1, current.y)).Vacant && unit.grid.GetTile(new Vector2Int(current.x, current.y + 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x - 1, current.y + 1)); // Top Left

        if (unit.grid.GetTile(new Vector2Int(current.x + 1, current.y)).Vacant && unit.grid.GetTile(new Vector2Int(current.x, current.y - 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x + 1, current.y - 1)); // Bottom Right

        if (unit.grid.GetTile(new Vector2Int(current.x - 1, current.y)).Vacant && unit.grid.GetTile(new Vector2Int(current.x, current.y - 1)).Vacant)
            neighbors.Add(new Vector2Int(current.x - 1, current.y - 1)); // Bottom Left


        return neighbors;
    }
    #endregion
    #endregion



    #endregion

    #region Mining
    public void Mine(Vector2Int target)
    {
        if (Vector3.Distance(unit.grid.GridToWorldPosition(target), transform.position) <= unit.reachRange)
        {

            if (!isMining)
            {
                StartCoroutine(MineCoroutine(target));
            }
        }
    }

    public IEnumerator MineCoroutine(Vector2Int target)
    {
        isMining = true;

        float miningTime = unit.grid.GetTile(target).Building.GetComponent<Mineable>().miningTime / unit.miningSpeed;
        while (miningTime > 0)
        {
            miningTime -= Time.deltaTime;
            yield return null;
        }
        unit.grid.GetTile(target).Destroy();
        isGoingToMine = false;
        isMining = false;
    }
    #endregion

    #region Combat

    public void Attack(GameObject target)
    {
        hasTarget = true;
        if (Vector3.Distance(target.transform.position, transform.position) <= unit.reachRange)
        {
            TurnTo(unit.grid.WorldToGridPosition(target.transform.position));
            unit.state = UnitState.ATTACKING;
            if (!isAttackOnCooldown)
            {
                StartCoroutine(attackCrt(target));
                
            }
        }
        else
        {
            movementTarget = unit.grid.WorldToGridPosition(target.transform.position);
            
        }
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
                if(diff > range)
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


    public IEnumerator attackCrt(GameObject target)
    {
        target.GetComponent<Unit>().TakeDmg(unit.attackDamage + unit.attackDamage * unit.PercentDamageBuff + unit.FlatDamageBuff);
        isAttackOnCooldown = true;
        yield return new WaitForSeconds(1.0f / unit.attackSpeed);
        isAttackOnCooldown = false;
    }

    #endregion

    #region Equipment

    #endregion

}
