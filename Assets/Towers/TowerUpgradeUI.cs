using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradePanel
    {
        public GameObject root;
        public TMP_Text titleText;
        public TMP_Text infoText;
        public Button upgradeButton;
        public TMP_Text upgradeButtonText;
    }

    public static TowerUpgradeUI Instance;

    [Header("Panels")]
    public GameObject rootPanel;
    public UpgradePanel currentPanel;
    public UpgradePanel nextPanel;
    public UpgradePanel finalPanel;

    private TowerBuff activeTower;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(TowerBuff tower)
    {
        if (tower == null)
            return;

        activeTower = tower;
        rootPanel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        activeTower = null;
    }

    public void Upgrade()
    {
        if (activeTower == null)
            return;

        if (activeTower.TryUpgrade())
            Refresh();
    }

    public void Refresh()
    {
        if (activeTower == null)
            return;

        TowerLevel[] levels = activeTower.levels;
        int currentIndex = activeTower.currentLevel;

        bool isFinal = levels == null || currentIndex >= levels.Length - 1;

        if (currentPanel.root != null)
            currentPanel.root.SetActive(!isFinal);
        if (nextPanel.root != null)
            nextPanel.root.SetActive(!isFinal);
        if (finalPanel.root != null)
            finalPanel.root.SetActive(isFinal);

        if (isFinal)
        {
            SetPanel(finalPanel, currentIndex + 1, levels[currentIndex], false);
            return;
        }

        SetPanel(currentPanel, currentIndex + 1, levels[currentIndex], true);
        SetPanel(nextPanel, currentIndex + 2, levels[currentIndex + 1], true);

        int cost = activeTower.NextUpgradeCost();
        SetCostText(currentPanel, cost);
        SetCostText(nextPanel, cost);
    }

    void SetPanel(UpgradePanel panel, int levelNumber, TowerLevel level, bool showButton)
    {
        if (panel == null)
            return;

        if (panel.titleText != null)
            panel.titleText.text = $"Level {levelNumber}";

        if (panel.infoText != null)
            panel.infoText.text = BuildLevelInfo(level);

        if (panel.upgradeButton != null)
            panel.upgradeButton.gameObject.SetActive(showButton);
    }

    void SetCostText(UpgradePanel panel, int cost)
    {
        if (panel == null || panel.upgradeButtonText == null)
            return;

        panel.upgradeButtonText.text = "Cost\n" + cost;
    }

    string BuildLevelInfo(TowerLevel lvl)
    {
        if (lvl == null)
            return "";

        System.Collections.Generic.List<string> lines = new System.Collections.Generic.List<string>();

        switch (lvl.effectType)
        {
            case TowerEffectType.NthHitDamage:
                AddLine(lines, $"Every {lvl.everyN} hits", lvl.everyN > 0);
                AddLine(lines, $"Damage x{lvl.multiplier}", lvl.multiplier > 0f);
                break;

            case TowerEffectType.ChainDamage:
                AddLine(lines, $"Every {lvl.chainNth} hits", lvl.chainNth > 0);
                AddLine(lines, $"Chain hits: {lvl.chainHits}", lvl.chainHits > 0);
                AddLine(lines, $"Chain damage: {lvl.chainDamage}", lvl.chainDamage > 0);
                AddLine(lines, $"Radius: {lvl.chainRadius}", lvl.chainRadius > 0f);
                break;

            case TowerEffectType.BombDamage:
                AddLine(lines, $"Every {lvl.bombNth} hits", lvl.bombNth > 0);
                AddLine(lines, $"Bomb damage: {lvl.bombDamage}", lvl.bombDamage > 0);
                AddLine(lines, $"Radius: {lvl.bombRadius}", lvl.bombRadius > 0f);
                break;

            case TowerEffectType.SlowEffect:
                AddLine(lines, $"Every {lvl.slowNth} hits", lvl.slowNth > 0);
                AddLine(lines, $"Slow: {FormatPercent(lvl.slowPower)}", lvl.slowPower > 0f);
                AddLine(lines, $"Radius: {lvl.slowRadius}", lvl.slowRadius > 0f);
                break;

            case TowerEffectType.MoneyEffect:
                AddLine(lines, $"Every {lvl.moneyNth} hits", lvl.moneyNth > 0);
                AddLine(lines, $"Money x{lvl.moneyMultiply}", lvl.moneyMultiply > 0);
                break;

            case TowerEffectType.ReverseEffect:
                AddLine(lines, $"Every {lvl.reverseNth} hits", lvl.reverseNth > 0);
                AddLine(lines, $"Reverse time: {lvl.reverseDuration}s", lvl.reverseDuration > 0f);
                break;
        }

        return string.Join("\n", lines);
    }

    string FormatPercent(float value)
    {
        float percent = value > 1f ? value : value * 100f;
        return $"{percent:0.#}%";
    }

    void AddLine(System.Collections.Generic.List<string> lines, string text, bool condition)
    {
        if (condition)
            lines.Add(text);
    }
}
