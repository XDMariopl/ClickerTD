using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TowerLevel))]
public class TowerLevelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float line = EditorGUIUtility.singleLineHeight;
        float pad = EditorGUIUtility.standardVerticalSpacing;

        Rect rect = new Rect(position.x, position.y, position.width, line);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        rect.y += line + pad;

        DrawField(ref rect, property, "effectType");
        DrawField(ref rect, property, "upgradeCost");

        TowerEffectType effect = (TowerEffectType)property.FindPropertyRelative("effectType").enumValueIndex;

        switch (effect)
        {
            case TowerEffectType.NthHitDamage:
                DrawField(ref rect, property, "everyN");
                DrawField(ref rect, property, "multiplier");
                break;

            case TowerEffectType.ChainDamage:
                DrawField(ref rect, property, "chainNth");
                DrawField(ref rect, property, "chainHits");
                DrawField(ref rect, property, "chainDamage");
                DrawField(ref rect, property, "chainRadius");
                break;

            case TowerEffectType.BombDamage:
                DrawField(ref rect, property, "bombNth");
                DrawField(ref rect, property, "bombDamage");
                DrawField(ref rect, property, "bombRadius");
                break;

            case TowerEffectType.SlowEffect:
                DrawField(ref rect, property, "slowNth");
                DrawField(ref rect, property, "slowPower");
                DrawField(ref rect, property, "slowRadius");
                break;

            case TowerEffectType.MoneyEffect:
                DrawField(ref rect, property, "moneyNth");
                DrawField(ref rect, property, "moneyMultiply");
                break;

            case TowerEffectType.ReverseEffect:
                DrawField(ref rect, property, "reverseNth");
                DrawField(ref rect, property, "reverseDuration");
                break;
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float pad = EditorGUIUtility.standardVerticalSpacing;

        if (!property.isExpanded)
            return line;

        float height = line + pad; // foldout line

        height += GetFieldHeight(property, "effectType") + pad;
        height += GetFieldHeight(property, "upgradeCost") + pad;

        TowerEffectType effect = (TowerEffectType)property.FindPropertyRelative("effectType").enumValueIndex;

        switch (effect)
        {
            case TowerEffectType.NthHitDamage:
                height += GetFieldHeight(property, "everyN") + pad;
                height += GetFieldHeight(property, "multiplier") + pad;
                break;
            case TowerEffectType.ChainDamage:
                height += GetFieldHeight(property, "chainNth") + pad;
                height += GetFieldHeight(property, "chainHits") + pad;
                height += GetFieldHeight(property, "chainDamage") + pad;
                height += GetFieldHeight(property, "chainRadius") + pad;
                break;
            case TowerEffectType.BombDamage:
                height += GetFieldHeight(property, "bombNth") + pad;
                height += GetFieldHeight(property, "bombDamage") + pad;
                height += GetFieldHeight(property, "bombRadius") + pad;
                break;
            case TowerEffectType.SlowEffect:
                height += GetFieldHeight(property, "slowNth") + pad;
                height += GetFieldHeight(property, "slowPower") + pad;
                height += GetFieldHeight(property, "slowRadius") + pad;
                break;
            case TowerEffectType.MoneyEffect:
                height += GetFieldHeight(property, "moneyNth") + pad;
                height += GetFieldHeight(property, "moneyMultiply") + pad;
                break;
            case TowerEffectType.ReverseEffect:
                height += GetFieldHeight(property, "reverseNth") + pad;
                height += GetFieldHeight(property, "reverseDuration") + pad;
                break;
        }

        return height;
    }

    private void DrawField(ref Rect rect, SerializedProperty property, string name)
    {
        SerializedProperty field = property.FindPropertyRelative(name);
        if (field == null)
            return;

        float h = EditorGUI.GetPropertyHeight(field, true);
        rect.height = h;
        EditorGUI.PropertyField(rect, field, true);
        rect.y += h + EditorGUIUtility.standardVerticalSpacing;
        rect.height = EditorGUIUtility.singleLineHeight;
    }

    private float GetFieldHeight(SerializedProperty property, string name)
    {
        SerializedProperty field = property.FindPropertyRelative(name);
        if (field == null)
            return 0f;

        return EditorGUI.GetPropertyHeight(field, true);
    }
}
