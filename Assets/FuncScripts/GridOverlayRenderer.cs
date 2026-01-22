using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class GridOverlayRenderer : MonoBehaviour
{
    public Color gridColor = new Color(1f, 1f, 1f, 0.6f); // alpha 150/255
    public float lineWidth = 0.02f;

    private GridManager grid;

    void Start()
    {
        grid = GetComponent<GridManager>();
        DrawGrid();
    }

    void DrawGrid()
    {
        //Vertical lines
        for (int x = 0; x <= grid.width; x++)
        {
            DrawLine(
                new Vector3(grid.origin.x + x * grid.cellSize, grid.origin.y),
                new Vector3(grid.origin.x + x * grid.cellSize, grid.origin.y + grid.height * grid.cellSize)
            );
        }

        //Horizontal lines
        for (int y = 0; y <= grid.height; y++)
        {
            DrawLine(
                new Vector3(grid.origin.x, grid.origin.y + y * grid.cellSize),
                new Vector3(grid.origin.x + grid.width * grid.cellSize, grid.origin.y + y * grid.cellSize)
            );
        }
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = gridColor;
        lr.endColor = gridColor;

        lr.sortingOrder = 0; 
    }
}
