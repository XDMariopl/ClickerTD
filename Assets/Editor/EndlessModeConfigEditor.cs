using UnityEditor;

[CustomEditor(typeof(EndlessModeConfig))]
public class EndlessModeConfigEditor : Editor
{
    SerializedProperty startingBudgetProp;
    SerializedProperty budgetIncreasePerRoundProp;
    SerializedProperty budgetRampEveryProp;
    SerializedProperty extraRampAmountProp;
    SerializedProperty delayBetweenRoundsProp;
    SerializedProperty enemyOptionsProp;
    SerializedProperty modifierOptionsProp;

    void OnEnable()
    {
        startingBudgetProp = serializedObject.FindProperty("startingBudget");
        budgetIncreasePerRoundProp = serializedObject.FindProperty("budgetIncreasePerRound");
        budgetRampEveryProp = serializedObject.FindProperty("budgetRampEvery");
        extraRampAmountProp = serializedObject.FindProperty("extraRampAmount");
        delayBetweenRoundsProp = serializedObject.FindProperty("delayBetweenRounds");
        enemyOptionsProp = serializedObject.FindProperty("enemyOptions");
        modifierOptionsProp = serializedObject.FindProperty("modifierOptions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(startingBudgetProp);
        EditorGUILayout.PropertyField(budgetIncreasePerRoundProp);
        EditorGUILayout.PropertyField(budgetRampEveryProp);
        EditorGUILayout.PropertyField(extraRampAmountProp);
        EditorGUILayout.PropertyField(delayBetweenRoundsProp);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(enemyOptionsProp, true);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(modifierOptionsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
