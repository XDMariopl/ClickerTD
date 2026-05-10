using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObstaclePath : MonoBehaviour
{
    public Transform[] Waypoints { get; private set; }

    [Header("Obstacle Settings")]
    public float width = 1f;
    public Color obstacleColor = new Color(0.25f, 0.25f, 0.25f, 0.9f);

    private LineRenderer lr;

    void Awake()
    {
        Waypoints = new Transform[transform.childCount];
        for (int i = 0; i < Waypoints.Length; i++)
            Waypoints[i] = transform.GetChild(i);

        lr = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        if (lr == null)
            return;

        lr.positionCount = Waypoints.Length;
        lr.useWorldSpace = true;

        for (int i = 0; i < Waypoints.Length; i++)
            lr.SetPosition(i, Waypoints[i].position);

        lr.startWidth = width;
        lr.endWidth = width;

        lr.startColor = obstacleColor;
        lr.endColor = obstacleColor;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 1;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = obstacleColor;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(
                transform.GetChild(i).position,
                transform.GetChild(i + 1).position
            );
        }
    }
#endif
}
