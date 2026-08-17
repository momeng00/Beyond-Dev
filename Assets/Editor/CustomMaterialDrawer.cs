using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static MaterialControl;

[CustomPropertyDrawer(typeof(CustomMaterial))]
public class CustomMaterialDrawer : PropertyDrawer
{

    //MaterialControl 및 SpriteMaterialControl이 인스펙터 창에서 이쁘게 보이게 만들기 위한 스크립트
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;
        float h;

        var nameProp = property.FindPropertyRelative("name");
        var typeProp = property.FindPropertyRelative("type");

        // name
        h = EditorGUI.GetPropertyHeight(nameProp);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), nameProp);
        y += h + 2;

        // type
        h = EditorGUI.GetPropertyHeight(typeProp);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), typeProp);
        y += h + 2;

        // value
        SerializedProperty valueProp = null;
        switch ((PropertyType)typeProp.enumValueIndex)
        {
            case PropertyType.Float: valueProp = property.FindPropertyRelative("floatValue"); break;
            case PropertyType.Boolean: valueProp = property.FindPropertyRelative("boolValue"); break;
            case PropertyType.Vector2: valueProp = property.FindPropertyRelative("vectorValue"); break;
        }

        if (valueProp != null)
        {
            h = EditorGUI.GetPropertyHeight(valueProp);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), valueProp);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = 0;
        float spacing = 2;

        var nameProp = property.FindPropertyRelative("name");
        var typeProp = property.FindPropertyRelative("type");

        h += EditorGUI.GetPropertyHeight(nameProp) + spacing;
        h += EditorGUI.GetPropertyHeight(typeProp) + spacing;

        SerializedProperty valueProp = null;
        switch ((PropertyType)typeProp.enumValueIndex)
        {
            case PropertyType.Float: valueProp = property.FindPropertyRelative("floatValue"); break;
            case PropertyType.Boolean: valueProp = property.FindPropertyRelative("boolValue"); break;
            case PropertyType.Vector2: valueProp = property.FindPropertyRelative("vectorValue"); break;
        }

        if (valueProp != null)
            h += EditorGUI.GetPropertyHeight(valueProp);

        return h;
    }
}