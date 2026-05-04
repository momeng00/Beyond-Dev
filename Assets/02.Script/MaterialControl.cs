using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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
        material = GetComponent<Image>().material;
    }
    private void Update()
    {
        Apply();
    }

    void OnValidate()
    {
        material = GetComponent<Image>().material;
        Apply();
    }

    public void Apply()
    {
        switch (firstMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(firstMaterial.name, firstMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(firstMaterial.name, firstMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(firstMaterial.name, firstMaterial.vectorValue);
                break;
        }

        switch (secondMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(secondMaterial.name, secondMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(secondMaterial.name, secondMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(secondMaterial.name, secondMaterial.vectorValue);
                break;
        }

        switch (thirdMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(thirdMaterial.name, thirdMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(thirdMaterial.name, thirdMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(thirdMaterial.name, thirdMaterial.vectorValue);
                break;
        }

        switch (fourthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(fourthMaterial.name, fourthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(fourthMaterial.name, fourthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(fourthMaterial.name, fourthMaterial.vectorValue);
                break;
        }
        switch (fifthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(fifthMaterial.name, fifthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(fifthMaterial.name, fifthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(fifthMaterial.name, fifthMaterial.vectorValue);
                break;
        }
        switch (sixthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(sixthMaterial.name, sixthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(sixthMaterial.name, sixthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(sixthMaterial.name, sixthMaterial.vectorValue);
                break;
        }
        switch (seventhMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(seventhMaterial.name, seventhMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(seventhMaterial.name, seventhMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(seventhMaterial.name, seventhMaterial.vectorValue);
                break;
        }
        switch (eighthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(eighthMaterial.name, eighthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(eighthMaterial.name, eighthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(eighthMaterial.name, eighthMaterial.vectorValue);
                break;
        }
        switch (ninthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(ninthMaterial.name, ninthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(ninthMaterial.name, ninthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(ninthMaterial.name, ninthMaterial.vectorValue);
                break;
        }
        switch (tenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(tenthMaterial.name, tenthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(tenthMaterial.name, tenthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(tenthMaterial.name, tenthMaterial.vectorValue);
                break;
        }
        switch (eleventhMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(eleventhMaterial.name, eleventhMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(eleventhMaterial.name, eleventhMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(eleventhMaterial.name, eleventhMaterial.vectorValue);
                break;
        }
        switch (twelfthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(twelfthMaterial.name, twelfthMaterial.floatValue);
                break;

            case PropertyType.Boolean:
                material.SetFloat(twelfthMaterial.name, twelfthMaterial.boolValue ? 1f : 0f);
                break;

            case PropertyType.Vector2:
                material.SetVector(twelfthMaterial.name, twelfthMaterial.vectorValue);
                break;
        }
    }
}