using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    public GridManager grid;
    public Path path;
    public ObstaclePath[] obstacles;

    void Start()
    {
        grid.BlockPathFromWaypoints(path.Waypoints);

        if (obstacles == null || obstacles.Length == 0)
            obstacles = FindObjectsByType<ObstaclePath>(FindObjectsSortMode.None);

        foreach (ObstaclePath obstacle in obstacles)
        {
            if (obstacle == null)
                continue;

            grid.BlockObstacleFromWaypoints(obstacle.Waypoints, obstacle.width);
        }
    }
}
