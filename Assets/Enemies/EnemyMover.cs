using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 2f;

    private Path path;
    private int waypointIndex = 0;

    public void Init(Path path)
    {
        this.path = path;
        transform.position = path.Waypoints[0].position;
    }

    void Update()
    {
        if (path == null) return;

        Transform target = path.Waypoints[waypointIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            waypointIndex++;

            if (waypointIndex >= path.Waypoints.Length)
            {
                ReachEnd();
            }
        }
    }

    void ReachEnd()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.TakeDamage(
                GetComponent<EnemyHealth>().damageToPlayer
            );
        }

        Destroy(gameObject);
    }
}
