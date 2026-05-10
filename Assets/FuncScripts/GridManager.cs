using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Settings")]
    public int width = 13;
    public int height = 10;
    public float cellSize = 1f;

    [Header("Grid Origin")]
    public Vector2 origin = Vector2.zero;

    private bool[,] pathCells;
    private bool[,] blockedCells;
    private bool[,] occupied;

    void Awake()
    {
        Instance = this;
        occupied = new bool[width, height];
        pathCells = new bool[width, height];
        blockedCells = new bool[width, height];
    }

    // ---------- WORLD GRID ----------

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(
            origin.x + gridPos.x * cellSize + cellSize / 2f,
            origin.y + gridPos.y * cellSize + cellSize / 2f
        );
    }

    public bool IsInsideGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.y >= 0 &&
               gridPos.x < width && gridPos.y < height;
    }

    public bool IsOccupied(Vector2Int gridPos)
    {
        return occupied[gridPos.x, gridPos.y] || pathCells[gridPos.x, gridPos.y] || blockedCells[gridPos.x, gridPos.y];
    }

    public void SetPath(Vector2Int gridPos)
    {
        pathCells[gridPos.x, gridPos.y] = true;
    }

    public void SetPlacementBlocked(Vector2Int gridPos)
    {
        blockedCells[gridPos.x, gridPos.y] = true;
    }

    public void SetOccupied(Vector2Int gridPos, bool value)
    {
        occupied[gridPos.x, gridPos.y] = value;
    }

    public void BlockPathFromWaypoints(Transform[] waypoints)
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector2 start = waypoints[i].position;
            Vector2 end = waypoints[i + 1].position;

            BlockLine(start, end);
        }
    }

    public void BlockObstacleFromWaypoints(Transform[] waypoints)
    {
        BlockObstacleFromWaypoints(waypoints, cellSize);
    }

    public void BlockObstacleFromWaypoints(Transform[] waypoints, float worldWidth)
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (waypoints.Length == 1)
        {
            BlockObstaclePoint(waypoints[0].position, worldWidth);
            return;
        }

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector2 start = waypoints[i].position;
            Vector2 end = waypoints[i + 1].position;

            BlockWideLine(start, end, worldWidth);
        }
    }

    void BlockObstaclePoint(Vector2 point, float worldWidth)
    {
        Vector2Int centerGrid = WorldToGrid(point);
        int radiusInCells = Mathf.Max(1, Mathf.CeilToInt((worldWidth * 0.5f) / cellSize));

        for (int x = centerGrid.x - radiusInCells; x <= centerGrid.x + radiusInCells; x++)
        {
            for (int y = centerGrid.y - radiusInCells; y <= centerGrid.y + radiusInCells; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (!IsInsideGrid(gridPos))
                    continue;

                Vector2 cellCenter = GridToWorld(gridPos);
                if (Vector2.Distance(cellCenter, point) > Mathf.Max(cellSize * 0.5f, worldWidth * 0.5f))
                    continue;

                SetPlacementBlocked(gridPos);
            }
        }
    }

    void BlockLine(Vector2 start, Vector2 end)
    {
        Vector2Int a = WorldToGrid(start);
        Vector2Int b = WorldToGrid(end);

        int dx = Mathf.Abs(b.x - a.x);
        int dy = Mathf.Abs(b.y - a.y);

        int sx = a.x < b.x ? 1 : -1;
        int sy = a.y < b.y ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (IsInsideGrid(a))
                SetPath(a);

            if (a == b)
                break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; a.x += sx; }
            if (e2 < dx) { err += dx; a.y += sy; }
        }
    }

    void BlockWideLine(Vector2 start, Vector2 end, float worldWidth)
    {
        Vector2 min = Vector2.Min(start, end) - Vector2.one * (worldWidth * 0.5f + cellSize);
        Vector2 max = Vector2.Max(start, end) + Vector2.one * (worldWidth * 0.5f + cellSize);

        Vector2Int minGrid = WorldToGrid(min);
        Vector2Int maxGrid = WorldToGrid(max);
        float radius = Mathf.Max(cellSize * 0.5f, worldWidth * 0.5f);

        for (int x = minGrid.x; x <= maxGrid.x; x++)
        {
            for (int y = minGrid.y; y <= maxGrid.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (!IsInsideGrid(gridPos))
                    continue;

                Vector2 cellCenter = GridToWorld(gridPos);
                float dist = DistancePointToSegment(cellCenter, start, end);
                if (dist > radius)
                    continue;

                SetPlacementBlocked(gridPos);
            }
        }
    }

    float DistancePointToSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float sqrLength = segment.sqrMagnitude;

        if (sqrLength <= Mathf.Epsilon)
            return Vector2.Distance(point, start);

        float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / sqrLength);
        Vector2 projection = start + segment * t;
        return Vector2.Distance(point, projection);
    }



    // ---------- DEBUG VISUAL ----------
    void OnDrawGizmos()
    {
        if (width <= 0 || height <= 0)
            return;

        Color gridColor = Color.gray;
        Color pathColor = new Color(0.5f, 0f, 0f, 0.6f);
        Color blockedColor = new Color(0.25f, 0.25f, 0.25f, 0.6f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 center = new Vector3(
                    origin.x + x * cellSize + cellSize / 2f,
                    origin.y + y * cellSize + cellSize / 2f,
                    0
                );

                Vector3 size = Vector3.one * cellSize;

                if (blockedCells != null && blockedCells[x, y])
                {
                    Gizmos.color = blockedColor;
                    Gizmos.DrawCube(center, size);
                }
                else if (pathCells != null && pathCells[x, y])
                {
                    Gizmos.color = pathColor;
                    Gizmos.DrawCube(center, size);
                }
            }
        }

        Gizmos.color = gridColor;

        for (int x = 0; x <= width; x++)
        {
            Vector3 from = new Vector3(origin.x + x * cellSize, origin.y, 0);
            Vector3 to = new Vector3(origin.x + x * cellSize, origin.y + height * cellSize, 0);
            Gizmos.DrawLine(from, to);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 from = new Vector3(origin.x, origin.y + y * cellSize, 0);
            Vector3 to = new Vector3(origin.x + width * cellSize, origin.y + y * cellSize, 0);
            Gizmos.DrawLine(from, to);
        }
    }


    public Vector2Int GetMouseGridPosition(Camera cam)
    {
        Vector2 world = cam.ScreenToWorldPoint(Input.mousePosition);
        return WorldToGrid(world);
    }

}
