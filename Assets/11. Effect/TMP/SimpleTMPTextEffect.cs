using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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

    [Header("Hangul Typing")]
    [Range(0f, 1f)] public float typingProgress = 1f;
    [TextArea(2, 6)] public string sourceText;

    private readonly List<string> typingFrames = new List<string>();
    private string cachedSource;
    private int appliedFrame = -1;
    private static readonly string[] Initials = {
        "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ",
        "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
    };
    private static readonly int[] FirstVowel = {
        -1,-1,-1,-1,-1,-1,-1,-1,-1,8,8,8,-1,-1,13,13,13,-1,-1,18,-1
    };
    private static readonly int[] FirstFinal = {
        -1,-1,-1,1,-1,4,4,-1,-1,8,8,8,8,8,8,8,-1,-1,17,-1,
        -1,-1,-1,-1,-1,-1,-1,-1
    };

    private TMP_Text tmp;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(sourceText)) sourceText = tmp.text;
    }

    private void LateUpdate()
    {
        ApplyTyping();
        ApplyEffects();
    }

    // Call this when a dialogue system supplies a new sentence.
    public void SetText(string text)
    {
        sourceText = text ?? string.Empty;
        cachedSource = null;
        appliedFrame = -1;
    }

    private static string Syllable(int initial, int vowel, int final = 0)
    {
        return ((char)(0xAC00 + initial * 588 + vowel * 28 + final)).ToString();
    }

    private void ApplyTyping()
    {
        string fullText = sourceText ?? string.Empty;
        if (cachedSource != fullText)
        {
            cachedSource = fullText;
            appliedFrame = -1;
            typingFrames.Clear();
            typingFrames.Add(string.Empty);
            string prefix = string.Empty;
            int position = 0;
            while (position < fullText.Length)
            {
                // Preserve TMP rich-text tags; sprite tags consume one step.
                if (tmp.richText && fullText[position] == '<')
                {
                    int end = fullText.IndexOf('>', position);
                    if (end >= 0)
                    {
                        string tag = fullText.Substring(position, end - position + 1);
                        prefix += tag;
                        position = end + 1;
                        if (tag.StartsWith("<sprite", System.StringComparison.OrdinalIgnoreCase))
                            AddFrame(prefix, fullText, position);
                        continue;
                    }
                }
                string element = StringInfo.GetNextTextElement(fullText, position);
                position += element.Length;
                int code = element[0] - 0xAC00;
                if (element.Length == 1 && code >= 0 && code < 11172)
                {
                    int initial = code / 588;
                    int vowel = code % 588 / 28;
                    int final = code % 28;
                    AddFrame(prefix + Initials[initial], fullText, position);
                    if (FirstVowel[vowel] >= 0)
                        AddFrame(prefix + Syllable(initial, FirstVowel[vowel]), fullText, position);
                    AddFrame(prefix + Syllable(initial, vowel), fullText, position);
                    if (final != 0)
                    {
                        if (FirstFinal[final] >= 0)
                            AddFrame(prefix + Syllable(initial, vowel, FirstFinal[final]), fullText, position);
                        AddFrame(prefix + element, fullText, position);
                    }
                }
                else AddFrame(prefix + element, fullText, position);
                prefix += element;
            }
            // Completion reproduces the exact original, including trailing tags.
            typingFrames[typingFrames.Count - 1] = fullText;
        }
        int frame = Mathf.FloorToInt(Mathf.Clamp01(typingProgress) * (typingFrames.Count - 1));
        if (frame == appliedFrame) return;
        appliedFrame = frame;
        tmp.maxVisibleCharacters = int.MaxValue;
        tmp.text = typingFrames[frame];
    }

    private void AddFrame(string text, string fullText, int position)
    {
        // Retain closing tags so partial text keeps its original formatting.
        if (tmp.richText)
            foreach (Match tag in Regex.Matches(fullText.Substring(position), @"</[^>]+>"))
                text += tag.Value;
        typingFrames.Add(text);
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

        // TMP mesh ����
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.vertices = meshInfo.vertices;
            meshInfo.mesh.colors32 = meshInfo.colors32;

            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}