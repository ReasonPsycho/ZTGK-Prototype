using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    private Unit unit;

    private ArrayList path;

    public Vector2Int target;
    public bool hasTarget = false;
    private bool isMoving = false;

    private void Start()
    {

        unit = GetComponentInParent<Unit>();
        path = new ArrayList();
    }

    private void Update()
    {
        if (hasTarget)
        {
            FindPathToTarget(target);
        }

        if (hasTarget && path == null)
        {
            target = FindNearestVacantTile(target);
        }


        if (path.Count != 0)
        {
            unit.state = UnitState.MOVING;

            MoveOnPath();
        }


        if (hasTarget && Vector3.Distance(transform.position, unit.grid.GridToWorldPosition(target)) < 0.05f)
        {
            hasTarget = false;
            isMoving = false;
        }

    }

    #region Movement


    public void MoveUnit(Vector2Int target)
    {
        StartCoroutine(MoveToCoroutine(target));
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
            path.RemoveAt(0);
            isMoving = false;
        }
        MoveUnit(nextTile);
    }


    public void FindPathToTarget(Vector2Int target)
    {
        // Clear the previous path
        path.Clear();

        // Get the current unit position
        Vector2Int startPosition = unit.grid.WorldToGridPosition(transform.position);

        // Perform A* pathfinding
        List<Vector2Int> waypoints = AStarSearch(startPosition, target);

        // If a path is found, store it in the path ArrayList
        if (waypoints != null)
        {
            foreach (Vector2Int waypoint in waypoints)
            {
                path.Add(waypoint);
            }
        }
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

    #endregion

    #region Combat

    #endregion

    #region Equipment

    #endregion

}
