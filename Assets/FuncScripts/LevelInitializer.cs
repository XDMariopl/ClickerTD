using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    public GridManager grid;
    public Path path;

    void Start()
    {
        grid.BlockPathFromWaypoints(path.Waypoints);
    }
}
