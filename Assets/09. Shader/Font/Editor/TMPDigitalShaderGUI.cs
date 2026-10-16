using UnityEditor;
using UnityEngine;

namespace Beyond.Editor
{
    // Keep the existing TMP inspector and expose the added digital texture controls.
    public sealed class TMPDigitalShaderGUI : TMPro.EditorUtilities.TMP_SDFShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Digital Texture", EditorStyles.boldLabel);
            Draw(materialEditor, properties, "_DigitalStrength", "Effect Strength");
            Draw(materialEditor, properties, "_DigitalChromaticPixels", "RGB Split (Screen Pixels)");
            Draw(materialEditor, properties, "_DigitalScanlineStrength", "Scanline Strength");
            Draw(materialEditor, properties, "_DigitalScanlineSpacing", "Scanline Spacing (Pixels)");
            Draw(materialEditor, properties, "_DigitalNoiseStrength", "Noise Strength");
            Draw(materialEditor, properties, "_DigitalFlickerStrength", "Flicker Strength");
            Draw(materialEditor, properties, "_DigitalEmissionStrength", "Emission Strength");
        }

        private static void Draw(MaterialEditor editor, MaterialProperty[] properties, string name, string label)
        {
            var property = FindProperty(name, properties, false);
            if (property != null) editor.ShaderProperty(property, new GUIContent(label));
        }
    }
}
