using UnityEngine;

public class TowerPlace : MonoBehaviour
{
    public GridManager grid;
    public Camera cam;

    private GameObject preview;
    private GameObject towerPrefab;
    private Vector2Int currentGridPos;
    private TowerData currentTower;
    private bool placing;

    public Transform towerParent;


    void Update()
    {
        if (!placing) return;

        currentGridPos = grid.GetMouseGridPosition(cam);

        if (!grid.IsInsideGrid(currentGridPos))
        {
            preview.SetActive(false);
            return;
        }

        preview.SetActive(true);
        preview.transform.position = grid.GridToWorld(currentGridPos);

        bool valid = !grid.IsOccupied(currentGridPos);
        SetPreviewColor(valid ? Color.green : Color.red);

        if (Input.GetMouseButtonDown(0) && valid)
            PlaceTower();

        if (Input.GetMouseButtonDown(1))
            Cancel();
    }

    public void StartPlacing(TowerData tower)
    {
        if (!MoneySystem.Instance.CanAfford(tower.cost))
        {
            Debug.Log("Not enough money!");
            return;
        }

        currentTower = tower;

        preview = Instantiate(tower.prefab, towerParent);

        foreach (var buff in preview.GetComponentsInChildren<TowerBuff>())
        {
            buff.isPreview = true;
        }

        SetPreviewAlpha(0.5f);
        placing = true;
    }


    void PlaceTower()
    {
        if (currentTower == null)
        {
            Debug.LogError("PlaceTower called but currentTower is null!");
            return;
        }

        if (!MoneySystem.Instance.SpendMoney(currentTower.cost))
        {
            Debug.Log("Not enough money to place tower!");
            return;
        }

        GameObject tower = Instantiate(currentTower.prefab, towerParent);
        tower.transform.position = grid.GridToWorld(currentGridPos);

        grid.SetOccupied(currentGridPos, true);

        Destroy(preview);
        placing = false;
    }


    void Cancel()
    {
        Destroy(preview);
        placing = false;
    }

    void SetPreviewAlpha(float a)
    {
        foreach (var r in preview.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = r.color;
            c.a = a;
            r.color = c;
        }
    }

    void SetPreviewColor(Color color)
    {
        foreach (var r in preview.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = color;
        }
    }
}
