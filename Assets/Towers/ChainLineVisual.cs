using UnityEngine;

public class ChainLineVisual : MonoBehaviour
{
    public float duration = 0.15f;

    private LineRenderer line;
    private float timer;

    public void Init(Vector3 start, Vector3 end)
    {
        line = gameObject.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.cyan;
        line.endColor = Color.white;

        line.useWorldSpace = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }
}
