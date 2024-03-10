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
    public bool hasTarget = false;

    private bool isMoving = false;


    public bool isGoingToMine = false;
    private bool isMining = false;
    protected void Start()
    {

        unit = GetComponentInParent<Unit>();
        path = new ArrayList();
    }

    private void Update()
    {
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
        
        else
        {
            if (moving != null)
            {
                StopCoroutine(moving);
                moving = null;
            }
            StopAllCoroutines();
        }


        if (hasTarget && Vector3.Distance(transform.position, unit.grid.GridToWorldPosition(movementTarget)) < 0.05f)
        {
            hasTarget = false;
            isMoving = false;
            if (moving != null)
            {
                StopCoroutine(moving);
                moving = null;
            }
         
            unit.state = UnitState.IDLE;
        }

        if (isGoingToMine)
        {
            Mine(miningTarget);
    
        }
        
        

    }

    #region Movement


    public void MoveUnit(Vector2Int target)
    {
        moving = StartCoroutine(MoveToCoroutine(target));
    }

    private IEnumerator MoveToCoroutine(Vector2Int target)
    {
        Vector3 targetPos = unit.grid.GridToWorldPosition(target);
        targetPos.y = transform.position.y;
        Vector3 startPos = unit.grid.GridToWorldPosition((Vector2Int)path[0]);
        startPos.y = transform.position.y;
        float t = 0;
        while (t < 1)
        {
            unit.animator.SetFloat("motionTime",t);
            t += Time.deltaTime * unit.tilesPerSecond;
            print(t);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            Vector3 dir = targetPos - transform.position;
            dir.y = 0; // Keep the direction in the XZ plane
            if (Vector3.Distance(Vector3.zero, dir) > 0.01f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), t *5);
            }
            //transform.rotation = Quaternion.Slerp(transform.rotation, tra, unit.rotationSpeed * Time.deltaTime);
            yield return null;
        }  
        
        isMoving = false;
    }

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

    public void MoveOnPath()
    {
        if (isMoving) return;
        isMoving = true;
        Vector2Int nextTile = (Vector2Int)path[0];
        if (path.Count > 1)
            nextTile = (Vector2Int)path[1];
        else
            isMoving = false;
        if (Vector3.Distance(transform.position, unit.grid.GridToWorldPosition(nextTile)) < 0.05f)
        {
            print("CALLLED");
            path.RemoveAt(0);
            isMoving = false;
        }
        MoveUnit(nextTile);
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

        float miningTime = unit.grid.GetTile(target).Building.GetComponent<Mineable>().miningTime / unit.miningSpeed ;
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

    #endregion

    #region Equipment

    #endregion

}
