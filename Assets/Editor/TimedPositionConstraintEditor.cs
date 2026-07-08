using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

[CustomEditor(typeof(TimedPositionConstraint))]
[CanEditMultipleObjects]
public sealed class TimedPositionConstraintEditor : Editor
{
    private SerializedProperty m_UpdateTiming;
    private SerializedProperty m_RenderCamera;
    private SerializedProperty m_ConstraintActive;
    private SerializedProperty m_Weight;
    private SerializedProperty m_Locked;
    private SerializedProperty m_TranslationAtRest;
    private SerializedProperty m_TranslationOffset;
    private SerializedProperty m_TranslationAxis;
    private SerializedProperty m_Sources;

    private void OnEnable()
    {
        m_UpdateTiming =
            serializedObject.FindProperty(
                "m_UpdateTiming");

        m_RenderCamera =
            serializedObject.FindProperty(
                "m_RenderCamera");

        m_ConstraintActive =
            serializedObject.FindProperty(
                "m_ConstraintActive");

        m_Weight =
            serializedObject.FindProperty(
                "m_Weight");

        m_Locked =
            serializedObject.FindProperty(
                "m_Locked");

        m_TranslationAtRest =
            serializedObject.FindProperty(
                "m_TranslationAtRest");

        m_TranslationOffset =
            serializedObject.FindProperty(
                "m_TranslationOffset");

        m_TranslationAxis =
            serializedObject.FindProperty(
                "m_TranslationAxis");

        m_Sources =
            serializedObject.FindProperty(
                "m_Sources");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bool activatePressed;
        bool zeroPressed;
        bool evaluatePressed = false;

        using (new EditorGUILayout.HorizontalScope())
        {
            activatePressed =
                GUILayout.Button("Activate");

            zeroPressed =
                GUILayout.Button("Zero");
        }

        EditorGUILayout.Space(3f);

        EditorGUILayout.PropertyField(
            m_ConstraintActive,
            new GUIContent("Is Active")
        );

        EditorGUILayout.PropertyField(
            m_Weight,
            new GUIContent("Weight")
        );

        EditorGUILayout.PropertyField(
            m_UpdateTiming,
            new GUIContent("Update Mode")
        );

        bool isBeforeRender =
            !m_UpdateTiming.hasMultipleDifferentValues &&
            m_UpdateTiming.enumValueIndex ==
            (int)TimedPositionConstraint.UpdateTiming
                .BeforeRender;

        if (isBeforeRender)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(
                m_RenderCamera,
                new GUIContent("Render Camera")
            );

            if (!m_RenderCamera
                    .hasMultipleDifferentValues &&
                m_RenderCamera
                    .objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "Render Camera가 비어 있으면 " +
                    "Sources의 Camera를 먼저 찾고, " +
                    "없으면 MainCamera를 사용합니다.",
                    MessageType.Info
                );
            }

            EditorGUI.indentLevel--;
        }

        bool isManual =
            !m_UpdateTiming.hasMultipleDifferentValues &&
            m_UpdateTiming.enumValueIndex ==
            (int)TimedPositionConstraint.UpdateTiming
                .Manual;

        if (isManual)
        {
            evaluatePressed =
                GUILayout.Button("Evaluate Now");
        }

        EditorGUILayout.Space(6f);

        EditorGUILayout.LabelField(
            "Constraint Settings",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            m_Locked,
            new GUIContent("Lock")
        );

        bool fieldsLocked =
            !m_Locked.hasMultipleDifferentValues &&
            m_Locked.boolValue;

        using (new EditorGUI.DisabledScope(fieldsLocked))
        {
            EditorGUILayout.PropertyField(
                m_TranslationAtRest,
                new GUIContent("Position At Rest")
            );

            EditorGUILayout.PropertyField(
                m_TranslationOffset,
                new GUIContent("Position Offset")
            );
        }

        DrawAxisField();

        EditorGUILayout.Space(6f);

        EditorGUILayout.PropertyField(
            m_Sources,
            new GUIContent("Sources"),
            true
        );

        bool propertiesChanged =
            serializedObject.ApplyModifiedProperties();

        if (activatePressed)
        {
            ApplyButtonAction(
                "Activate Position Constraint",
                constraint => constraint.Activate()
            );
        }
        else if (zeroPressed)
        {
            ApplyButtonAction(
                "Zero Position Constraint",
                constraint => constraint.Zero()
            );
        }
        else if (evaluatePressed)
        {
            ApplyButtonAction(
                "Evaluate Position Constraint",
                constraint => constraint.EvaluateNow()
            );
        }
        else if (propertiesChanged)
        {
            EvaluateSelectedObjects();
        }
    }

    private void DrawAxisField()
    {
        EditorGUI.showMixedValue =
            m_TranslationAxis.hasMultipleDifferentValues;

        Axis currentAxis =
            (Axis)m_TranslationAxis.intValue;

        EditorGUI.BeginChangeCheck();

        Axis newAxis =
            (Axis)EditorGUILayout.EnumFlagsField(
                new GUIContent(
                    "Freeze Position Axes"),
                currentAxis
            );

        if (EditorGUI.EndChangeCheck())
        {
            m_TranslationAxis.intValue =
                (int)newAxis;
        }

        EditorGUI.showMixedValue = false;
    }

    private void ApplyButtonAction(
        string undoName,
        Action<TimedPositionConstraint> action)
    {
        foreach (UnityEngine.Object selectedTarget
                 in targets)
        {
            TimedPositionConstraint constraint =
                (TimedPositionConstraint)selectedTarget;

            Undo.RecordObject(
                constraint,
                undoName
            );

            Undo.RecordObject(
                constraint.transform,
                undoName
            );

            action(constraint);

            EditorUtility.SetDirty(constraint);
            EditorUtility.SetDirty(
                constraint.transform);
        }

        serializedObject.Update();

        SceneView.RepaintAll();
        Repaint();
    }

    private void EvaluateSelectedObjects()
    {
        foreach (UnityEngine.Object selectedTarget
                 in targets)
        {
            TimedPositionConstraint constraint =
                (TimedPositionConstraint)selectedTarget;

            Undo.RecordObject(
                constraint.transform,
                "Evaluate Position Constraint"
            );

            constraint.EvaluateNow();

            EditorUtility.SetDirty(constraint);
            EditorUtility.SetDirty(
                constraint.transform);
        }

        SceneView.RepaintAll();
    }
}