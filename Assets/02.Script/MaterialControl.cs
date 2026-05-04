using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MaterialControl : MonoBehaviour
{
    private Material material;
    [Serializable]
    public struct CustomMaterial
    {
        public string name;

        public PropertyType type;

        public Vector2 vectorValue;
        public float floatValue;
        public bool boolValue;
    }
    [Header("First")]
    public CustomMaterial firstMaterial;
    [Header("Second")]
    public CustomMaterial secondMaterial;
    [Header("Third")]
    public CustomMaterial thirdMaterial;
    [Header("Fourth")]
    public CustomMaterial fourthMaterial;
    [Header("Fifth")]
    public CustomMaterial fifthMaterial;
    [Header("Sixth")]
    public CustomMaterial sixthMaterial;
    [Header("Seventh")]
    public CustomMaterial seventhMaterial;
    [Header("Eighth")]
    public CustomMaterial eighthMaterial;
    [Header("Ninth")]
    public CustomMaterial ninthMaterial;
    [Header("Tenth")]
    public CustomMaterial tenthMaterial;
    [Header("Eleventh")]
    public CustomMaterial eleventhMaterial;
    [Header("Twelfth")]
    public CustomMaterial twelfthMaterial;

    private void Awake()
    {
        material = GetComponent<Material>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        switch (firstMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, firstMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, firstMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, firstMaterial.vectorValue);
                break;
        }

        switch (secondMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, secondMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, secondMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, secondMaterial.vectorValue);
                break;
        }

        switch (thirdMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, thirdMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, thirdMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, thirdMaterial.vectorValue);
                break;
        }

        switch (fourthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, fourthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, fourthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, fourthMaterial.vectorValue);
                break;
        }
        switch (fifthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, fifthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, fifthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, fifthMaterial.vectorValue);
                break;
        }
        switch (sixthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, sixthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, sixthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, sixthMaterial.vectorValue);
                break;
        }
        switch (seventhMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, seventhMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, seventhMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, seventhMaterial.vectorValue);
                break;
        }
        switch (eighthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, eighthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, eighthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, eighthMaterial.vectorValue);
                break;
        }
        switch (ninthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, ninthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, ninthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, ninthMaterial.vectorValue);
                break;
        }
        switch (tenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, tenthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, tenthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, tenthMaterial.vectorValue);
                break;
        }
        switch (eleventhMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, eleventhMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, eleventhMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, eleventhMaterial.vectorValue);
                break;
        }
        switch (twelfthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(name, twelfthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(name, twelfthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(name, twelfthMaterial.vectorValue);
                break;
        }
    }
#endif
}