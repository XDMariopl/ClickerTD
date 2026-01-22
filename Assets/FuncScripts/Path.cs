using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Path : MonoBehaviour
{
    public Transform[] Waypoints { get; private set; }

    [Header("In-Game Path Visual")]
    public float width = 1f;
    public Color pathColor = new Color(0.5f, 0f, 0f, 150f / 255f);

    LineRenderer lr;

    void Awake()
    {
        // Cache waypoints
        Waypoints = new Transform[transform.childCount];
        for (int i = 0; i < Waypoints.Length; i++)
            Waypoints[i] = transform.GetChild(i);

        // Setup renderer
        lr = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        lr.positionCount = Waypoints.Length;
        lr.useWorldSpace = true;

        for (int i = 0; i < Waypoints.Length; i++)
            lr.SetPosition(i, Waypoints[i].position);

        lr.startWidth = width;
        lr.endWidth = width;

        lr.startColor = pathColor;
        lr.endColor = pathColor;

        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.sortingLayerName = "Default";
        lr.sortingOrder = 1;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
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
