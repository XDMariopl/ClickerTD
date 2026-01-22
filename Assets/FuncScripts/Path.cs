using UnityEngine;

public class Path : MonoBehaviour
{
    public Transform[] Waypoints { get; private set; }

    void Awake()
    {
        Waypoints = new Transform[transform.childCount];
        for (int i = 0; i < Waypoints.Length; i++)
        {
            Waypoints[i] = transform.GetChild(i);
        }
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
