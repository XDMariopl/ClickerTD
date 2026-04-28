using UnityEngine;

[CreateAssetMenu(menuName = "TD/Tower")]
public class TowerData : ScriptableObject
{
    public string towerId;
    public GameObject prefab;
    public int cost = 0;
    public int unlockAfterCompletedLevels = 0;
}
