using System;
using UnityEngine;
using UnityEngine.UI;
public enum PropertyType
{
    Color,
    Texture2D,
    Texture2DArray,
    Texture3D,
    Cubemap,
    Gradient,
    Boolean,
    Float,
    Vector2,
    Vector3,
    Vector4,
    Matrix2,
    Matrix3,
    Matrix4,
    SamplerState,
    VirtualTexture,
    PropertyConnectionState
}
public class MaterialControl : MonoBehaviour
{
    //PropertyType가 변경이 되면 설정값들이 다 털리는데 좋은 방안이 없을까?
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
    [Header("thirteenth")]
    public CustomMaterial thirteenthMaterial;
    [Header("fourteenth")]
    public CustomMaterial fourteenthMaterial;
    [Header("fifteenth")]
    public CustomMaterial fifteenthMaterial;
    [Header("sixteenth")]
    public CustomMaterial sixteenthMaterial;
    [Header("seventeenth")]
    public CustomMaterial seventeenthMaterial;
    [Header("eighteenth")]
    public CustomMaterial eighteenthMaterial;
    [Header("eighteenth")]
    public CustomMaterial nineteenthMaterial;
    [Header("twentieth")]
    public CustomMaterial twentiethMaterial;

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
        switch (thirteenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(thirteenthMaterial.name, thirteenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(thirteenthMaterial.name, thirteenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(thirteenthMaterial.name, thirteenthMaterial.vectorValue);
                break;
        }
        switch (fourteenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(fourteenthMaterial.name, fourteenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(fourteenthMaterial.name, fourteenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(fourteenthMaterial.name, fourteenthMaterial.vectorValue);
                break;
        }
        switch (fifteenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(fifteenthMaterial.name, fifteenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(fifteenthMaterial.name, fifteenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(fifteenthMaterial.name, fifteenthMaterial.vectorValue);
                break;
        }
        switch (sixteenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(sixteenthMaterial.name, sixteenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(sixteenthMaterial.name, sixteenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(sixteenthMaterial.name, sixteenthMaterial.vectorValue);
                break;
        }
        switch (seventeenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(seventeenthMaterial.name, seventeenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(seventeenthMaterial.name, seventeenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(seventeenthMaterial.name, seventeenthMaterial.vectorValue);
                break;
        }
        switch (eighteenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(eighteenthMaterial.name, eighteenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(eighteenthMaterial.name, eighteenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(eighteenthMaterial.name, eighteenthMaterial.vectorValue);
                break;
        }
        switch (nineteenthMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(nineteenthMaterial.name, nineteenthMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(nineteenthMaterial.name, nineteenthMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(nineteenthMaterial.name, nineteenthMaterial.vectorValue);
                break;
        }
        switch (twentiethMaterial.type)
        {
            case PropertyType.Float:
                material.SetFloat(twentiethMaterial.name, twentiethMaterial.floatValue);
                break;
            case PropertyType.Boolean:
                material.SetFloat(twentiethMaterial.name, twentiethMaterial.boolValue ? 1f : 0f);
                break;
            case PropertyType.Vector2:
                material.SetVector(twentiethMaterial.name, twentiethMaterial.vectorValue);
                break;
        }
    }
}