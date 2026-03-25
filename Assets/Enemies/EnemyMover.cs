using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 2f;

    private Path path;
    private int waypointIndex = 0;
    private float speedMultiplier = 1f;
    private float reverseTimer = 0f;

    public void Init(Path path)
    {
        this.path = path;
        transform.position = path.Waypoints[0].position;
    }

    void Update()
    {
        if (path == null) return;

        float dt = Time.deltaTime;

        if (reverseTimer > 0f)
            reverseTimer -= dt;

        int direction = reverseTimer > 0f ? -1 : 1;
        int targetIndex = direction == 1 ? waypointIndex : Mathf.Max(waypointIndex - 1, 0);

        Transform target = path.Waypoints[targetIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * speedMultiplier * dt
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            if (direction == 1)
            {
                waypointIndex++;

                if (waypointIndex >= path.Waypoints.Length)
                    ReachEnd();
            }
            else
            {
                waypointIndex = Mathf.Max(waypointIndex - 1, 0);
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

    public void ApplySlow(float slowPercent)
    {
        float normalized = slowPercent > 1f ? slowPercent / 100f : slowPercent;
        normalized = Mathf.Clamp01(normalized);
        float newMultiplier = 1f - normalized;

        if (newMultiplier < speedMultiplier)
            speedMultiplier = newMultiplier;
    }

    public void ApplyReverse(float duration)
    {
        if (duration <= 0f) return;
        reverseTimer = Mathf.Max(reverseTimer, duration);
    }
}
