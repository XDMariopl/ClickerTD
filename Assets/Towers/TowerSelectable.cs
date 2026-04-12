using UnityEngine;

public class TowerSelectable : MonoBehaviour
{
    private TowerBuff towerBuff;

    void Awake()
    {
        towerBuff = GetComponentInChildren<TowerBuff>();
    }

    void OnMouseDown()
    {
        if (TowerUpgradeUI.Instance == null || towerBuff == null)
            return;

        TowerUpgradeUI.Instance.Show(towerBuff);
    }
}
