using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InteractAction))]
public class InteractActionDrawer : PropertyDrawer
{
    private const float Spacing = 2f;

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        // Action 이름
        Rect foldoutRect = new Rect(
            position.x,
            y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        property.isExpanded = EditorGUI.Foldout(
            foldoutRect,
            property.isExpanded,
            label
        );

        y += EditorGUIUtility.singleLineHeight + Spacing;

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        // Type
        SerializedProperty typeProp =
            property.FindPropertyRelative("type");

        float height = EditorGUI.GetPropertyHeight(typeProp);

        EditorGUI.PropertyField(
            new Rect(position.x, y, position.width, height),
            typeProp
        );

        y += height + Spacing;

        // 현재 Type에 맞는 Value와 Event만 표시
        switch ((InteractValueType)typeProp.enumValueIndex)
        {
            case InteractValueType.Bool:

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("boolValue"),
                    "Value"
                );

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("onBool"),
                    "Event"
                );

                break;


            case InteractValueType.Int:

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("intValue"),
                    "Value"
                );

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("onInt"),
                    "Event"
                );

                break;


            case InteractValueType.Float:

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("floatValue"),
                    "Value"
                );

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("onFloat"),
                    "Event"
                );

                break;


            case InteractValueType.String:

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("stringValue"),
                    "Value"
                );

                DrawProperty(
                    ref y,
                    position,
                    property.FindPropertyRelative("onString"),
                    "Event"
                );

                break;
        }

        EditorGUI.EndProperty();
    }


    private void DrawProperty(
        ref float y,
        Rect position,
        SerializedProperty property,
        string label)
    {
        float height = EditorGUI.GetPropertyHeight(
            property,
            true
        );

        EditorGUI.PropertyField(
            new Rect(
                position.x,
                y,
                position.width,
                height
            ),
            property,
            new GUIContent(label),
            true
        );

        y += height + Spacing;
    }


    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        float height =
            EditorGUIUtility.singleLineHeight + Spacing;

        // 접혀있으면 Foldout 높이만 반환
        if (!property.isExpanded)
            return height;

        SerializedProperty typeProp =
            property.FindPropertyRelative("type");

        height +=
            EditorGUI.GetPropertyHeight(typeProp) +
            Spacing;

        switch ((InteractValueType)typeProp.enumValueIndex)
        {
            case InteractValueType.Bool:

                height += GetPropertyHeight(
                    property,
                    "boolValue"
                );

                height += GetPropertyHeight(
                    property,
                    "onBool"
                );

                break;


            case InteractValueType.Int:

                height += GetPropertyHeight(
                    property,
                    "intValue"
                );

                height += GetPropertyHeight(
                    property,
                    "onInt"
                );

                break;


            case InteractValueType.Float:

                height += GetPropertyHeight(
                    property,
                    "floatValue"
                );

                height += GetPropertyHeight(
                    property,
                    "onFloat"
                );

                break;


            case InteractValueType.String:

                height += GetPropertyHeight(
                    property,
                    "stringValue"
                );

                height += GetPropertyHeight(
                    property,
                    "onString"
                );

                break;
        }

        return height;
    }


    private float GetPropertyHeight(
        SerializedProperty parent,
        string propertyName)
    {
        SerializedProperty property =
            parent.FindPropertyRelative(propertyName);

        return EditorGUI.GetPropertyHeight(
            property,
            true
        ) + Spacing;
    }
}