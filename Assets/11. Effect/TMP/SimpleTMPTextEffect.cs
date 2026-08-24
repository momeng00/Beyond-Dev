using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class SimpleTMPTextEffect : MonoBehaviour
{
    [Header("Effects")]
    public bool wave;
    public bool shake;
    public bool rainbow;

    [Header("Wave")]
    public float waveHeight = 5f;
    public float waveSpeed = 5f;
    public float waveSpacing = 0.5f;

    [Header("Shake")]
    public float shakeAmount = 1f;
    public float shakeSpeed = 20f;

    [Header("Rainbow")]
    public float rainbowSpeed = 0.5f;
    public float rainbowSpacing = 0.08f;

    private TMP_Text tmp;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        ApplyEffects();
    }

    private void ApplyEffects()
    {
        tmp.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];

            if (!character.isVisible)
                continue;

            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            Vector3 offset = Vector3.zero;

            // Wave
            if (wave)
            {
                float y =
                    Mathf.Sin(
                        Time.time * waveSpeed +
                        i * waveSpacing
                    ) * waveHeight;

                offset.y += y;
            }

            // Shake
            if (shake)
            {
                float time = Time.time * shakeSpeed;

                float x = Mathf.PerlinNoise(i * 10f, time) * 2f - 1f;
                float y = Mathf.PerlinNoise(i * 20f, time) * 2f - 1f;

                offset += new Vector3(x, y, 0f) * shakeAmount;
            }

            // Apply position
            for (int j = 0; j < 4; j++)
                vertices[vertexIndex + j] += offset;

            // Rainbow
            if (rainbow)
            {
                float hue = Mathf.Repeat(
                    Time.time * rainbowSpeed +
                    i * rainbowSpacing,
                    1f
                );

                Color color = Color.HSVToRGB(hue, 1f, 1f);

                for (int j = 0; j < 4; j++)
                    colors[vertexIndex + j] = color;
            }
        }

        // TMP mesh °»½Å
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.vertices = meshInfo.vertices;
            meshInfo.mesh.colors32 = meshInfo.colors32;

            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}